"""Train stroke model with stronger imbalance handling and threshold tuning."""

from pathlib import Path
import warnings

import joblib
import numpy as np
import pandas as pd
from imblearn.combine import SMOTEENN, SMOTETomek
from imblearn.over_sampling import SMOTE
from imblearn.pipeline import Pipeline as ImbPipeline
from sklearn.calibration import CalibratedClassifierCV
from sklearn.base import clone
from sklearn.ensemble import GradientBoostingClassifier, RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import (
    average_precision_score,
    classification_report,
    confusion_matrix,
    f1_score,
    make_scorer,
    precision_recall_curve,
    precision_score,
    recall_score,
    roc_auc_score,
)
from sklearn.model_selection import StratifiedKFold, cross_val_predict, cross_validate, train_test_split
from sklearn.preprocessing import StandardScaler

warnings.filterwarnings("ignore")

print("=" * 70)
print("🚀 STROKE MODEL TRAINING (ADVANCED IMBALANCE HANDLING)")
print("=" * 70)

base_dir = Path(__file__).resolve().parent
data_path = base_dir / "data" / "stroke.csv"
if not data_path.exists():
    raise FileNotFoundError(f"Không tìm thấy dataset: {data_path}")

print(f"\n📂 Loading dataset: {data_path}")
df = pd.read_csv(data_path)
print(f"✅ Rows: {len(df)}, Columns: {len(df.columns)}")

required_columns = [
    "gender",
    "age",
    "hypertension",
    "heart_disease",
    "avg_glucose_level",
    "bmi",
    "smoking_status",
    "stroke",
]
missing_columns = [col for col in required_columns if col not in df.columns]
if missing_columns:
    raise ValueError(f"Thiếu cột bắt buộc: {missing_columns}")

work = df.copy()
work = work[work["gender"].isin(["Male", "Female"])]
work["bmi"] = work["bmi"].fillna(work["bmi"].median())

smoking_map = {
    "never smoked": 0,
    "Unknown": 0,
    "formerly smoked": 1,
    "smokes": 1,
}

X = pd.DataFrame()
X["Age"] = work["age"]
X["Gender"] = (work["gender"] == "Male").astype(int)
X["Hypertension"] = work["hypertension"].astype(int)
X["HeartDisease"] = work["heart_disease"].astype(int)
X["Glucose"] = work["avg_glucose_level"]
X["BMI"] = work["bmi"]
X["Smoking"] = work["smoking_status"].map(smoking_map).fillna(0).astype(int)

y = work["stroke"].astype(int)
feature_names = list(X.columns)

print("\n📊 Class distribution (value_counts):")
print(y.value_counts().sort_index())
print((y.value_counts(normalize=True).sort_index() * 100).round(2).rename("ratio_%"))

X_train, X_test, y_train, y_test = train_test_split(
    X,
    y,
    test_size=0.2,
    random_state=42,
    stratify=y,
)

print(f"\n✂️ Train size: {len(X_train)}, Test size: {len(X_test)}")
print("📊 Train class distribution:")
print(y_train.value_counts().sort_index())

cv = StratifiedKFold(n_splits=5, shuffle=True, random_state=42)

resamplers = {
    "None": None,
    "SMOTE_0.4": SMOTE(sampling_strategy=0.4, random_state=42),
    "SMOTE_0.6": SMOTE(sampling_strategy=0.6, random_state=42),
    "SMOTE_1.0": SMOTE(sampling_strategy=1.0, random_state=42),
    "SMOTETomek_0.6": SMOTETomek(sampling_strategy=0.6, random_state=42),
    "SMOTEENN_0.6": SMOTEENN(sampling_strategy=0.6, random_state=42),
}

scale_pos_weight = max((y_train == 0).sum() / max(1, (y_train == 1).sum()), 1.0)

models = {
    "LogisticRegression": LogisticRegression(
        max_iter=5000,
        random_state=42,
        class_weight="balanced",
    ),
    "RandomForest": RandomForestClassifier(
        n_estimators=700,
        min_samples_leaf=2,
        random_state=42,
        class_weight="balanced",
        n_jobs=-1,
    ),
    "GradientBoosting": GradientBoostingClassifier(random_state=42),
}

try:
    from xgboost import XGBClassifier

    models["XGBoost"] = XGBClassifier(
        n_estimators=250,
        learning_rate=0.05,
        max_depth=4,
        subsample=0.9,
        colsample_bytree=0.9,
        reg_lambda=1.0,
        min_child_weight=2,
        objective="binary:logistic",
        eval_metric="logloss",
        random_state=42,
        n_jobs=-1,
        tree_method="hist",
        scale_pos_weight=scale_pos_weight,
    )
    print("✅ XGBoost available: included in model comparison")
except Exception:
    print("⚠️ XGBoost not available: skipped")


def build_pipeline(resampler, estimator):
    steps = [("scaler", StandardScaler())]
    if resampler is not None:
        steps.append(("resampler", clone(resampler)))
    steps.append(("model", clone(estimator)))
    return ImbPipeline(steps=steps)


def build_calibrator(model, method='sigmoid'):
    try:
        from sklearn.frozen import FrozenEstimator

        frozen_model = FrozenEstimator(model)
        try:
            return CalibratedClassifierCV(estimator=frozen_model, method=method)
        except TypeError:
            return CalibratedClassifierCV(base_estimator=frozen_model, method=method)
    except Exception:
        pass

    try:
        return CalibratedClassifierCV(estimator=model, method=method, cv='prefit')
    except TypeError:
        return CalibratedClassifierCV(base_estimator=model, method=method, cv='prefit')


def evaluate_threshold_grid(y_true, y_prob, thresholds):
    rows = []
    for threshold in thresholds:
        y_pred = (y_prob >= threshold).astype(int)
        rows.append(
            {
                "Threshold": float(threshold),
                "Precision": precision_score(y_true, y_pred, zero_division=0),
                "Recall": recall_score(y_true, y_pred, zero_division=0),
                "F1": f1_score(y_true, y_pred, zero_division=0),
            }
        )
    return pd.DataFrame(rows)


print("\n🔍 Cross-validation comparison (StratifiedKFold)...")

scoring = {
    "f1": make_scorer(f1_score, zero_division=0),
    "precision": make_scorer(precision_score, zero_division=0),
    "recall": make_scorer(recall_score, zero_division=0),
    "roc_auc": "roc_auc",
    "pr_auc": "average_precision",
}

cv_rows = []
for resampler_name, resampler in resamplers.items():
    for model_name, estimator in models.items():
        pipeline = build_pipeline(resampler, estimator)
        cv_result = cross_validate(
            pipeline,
            X_train,
            y_train,
            cv=cv,
            scoring=scoring,
            n_jobs=1,
            return_train_score=False,
        )

        row = {
            "Resampler": resampler_name,
            "Model": model_name,
            "CV_F1": float(np.mean(cv_result["test_f1"])),
            "CV_Precision": float(np.mean(cv_result["test_precision"])),
            "CV_Recall": float(np.mean(cv_result["test_recall"])),
            "CV_ROC_AUC": float(np.mean(cv_result["test_roc_auc"])),
            "CV_PR_AUC": float(np.mean(cv_result["test_pr_auc"])),
        }
        row["SelectionScore"] = (0.6 * row["CV_PR_AUC"]) + (0.4 * row["CV_F1"])
        cv_rows.append(row)

cv_df = pd.DataFrame(cv_rows).sort_values(
    by=["SelectionScore", "CV_PR_AUC", "CV_F1", "CV_Precision"],
    ascending=False,
)

print("\n📊 CV comparison (top 12):")
print(cv_df.head(12).to_string(index=False, float_format=lambda v: f"{v:.4f}"))

best_row = cv_df.iloc[0]
best_resampler_name = best_row["Resampler"]
best_model_name = best_row["Model"]
best_resampler = resamplers[best_resampler_name]
best_model_template = models[best_model_name]

print(
    f"\n🏆 Best CV candidate: {best_model_name} + {best_resampler_name} "
    f"(SelectionScore={best_row['SelectionScore']:.4f})"
)

best_pipeline = build_pipeline(best_resampler, best_model_template)
oof_prob = cross_val_predict(
    best_pipeline,
    X_train,
    y_train,
    cv=cv,
    method="predict_proba",
    n_jobs=1,
)[:, 1]

threshold_grid = np.round(
    np.concatenate([
        np.arange(0.01, 0.21, 0.01),
        np.arange(0.25, 0.901, 0.05),
    ]),
    2,
)
min_recall = 0.40

cv_threshold_table = evaluate_threshold_grid(y_train, oof_prob, threshold_grid)
cv_threshold_table["RecallEligible"] = cv_threshold_table["Recall"] >= min_recall

print("\n📈 Threshold comparison table (CV OOF, 0.3 -> 0.9):")
print(cv_threshold_table.to_string(index=False, float_format=lambda v: f"{v:.4f}"))

eligible_rows = cv_threshold_table[cv_threshold_table["RecallEligible"]].copy()
if not eligible_rows.empty:
    best_threshold_row = eligible_rows.sort_values(
        by=["Precision", "F1", "Recall", "Threshold"],
        ascending=[False, False, False, True],
    ).iloc[0]
else:
    best_threshold_row = cv_threshold_table.sort_values(
        by=["Recall", "Precision", "F1", "Threshold"],
        ascending=[False, False, False, True],
    ).iloc[0]

best_threshold = float(best_threshold_row["Threshold"])
oof_best_threshold = best_threshold

print(
    f"\n🎯 Precision-priority threshold selected: {best_threshold:.2f} "
    f"(precision={best_threshold_row['Precision']:.4f}, recall={best_threshold_row['Recall']:.4f}, "
    f"f1={best_threshold_row['F1']:.4f}, recall>=0.40={bool(best_threshold_row['RecallEligible'])})"
)

X_fit, X_cal, y_fit, y_cal = train_test_split(
    X_train,
    y_train,
    test_size=0.2,
    random_state=42,
    stratify=y_train,
)

print(f"\n🧪 Calibration split: fit={len(X_fit)}, calibration={len(X_cal)}")

scaler = StandardScaler()
X_fit_scaled = scaler.fit_transform(X_fit)
X_cal_scaled = scaler.transform(X_cal)
X_test_scaled = scaler.transform(X_test)

if best_resampler is not None:
    X_train_balanced, y_train_balanced = clone(best_resampler).fit_resample(X_fit_scaled, y_fit)
    print("\n⚖️ Final training class distribution after resampling:")
    print(pd.Series(y_train_balanced).value_counts().sort_index())
else:
    X_train_balanced, y_train_balanced = X_fit_scaled, y_fit
    print("\n✅ Final training without additional resampling")

final_model = clone(best_model_template)
final_model.fit(X_train_balanced, y_train_balanced)

deployed_model = final_model
calibration_enabled = False
calibration_method = 'sigmoid'

if len(pd.Series(y_cal).unique()) > 1:
    try:
        calibrator = build_calibrator(final_model, method=calibration_method)
        calibrator.fit(X_cal_scaled, y_cal)
        deployed_model = calibrator
        calibration_enabled = True
        print(f"✅ Probability calibration enabled ({calibration_method})")
    except Exception as calibrate_err:
        print(f"⚠️ Calibration failed. Continue with uncalibrated model: {calibrate_err}")
else:
    print("⚠️ Calibration skipped: calibration split does not contain both classes")

calibration_threshold_table = evaluate_threshold_grid(
    y_cal,
    deployed_model.predict_proba(X_cal_scaled)[:, 1],
    threshold_grid,
)
calibration_threshold_table["RecallEligible"] = calibration_threshold_table["Recall"] >= min_recall

eligible_calibration_rows = calibration_threshold_table[calibration_threshold_table["RecallEligible"]].copy()
if not eligible_calibration_rows.empty:
    best_threshold_row = eligible_calibration_rows.sort_values(
        by=["Precision", "F1", "Recall", "Threshold"],
        ascending=[False, False, False, True],
    ).iloc[0]
else:
    best_threshold_row = calibration_threshold_table.sort_values(
        by=["Recall", "Precision", "F1", "Threshold"],
        ascending=[False, False, False, True],
    ).iloc[0]

best_threshold = float(best_threshold_row["Threshold"])

print("\n📈 Threshold comparison table (calibration split, 0.3 -> 0.9):")
print(calibration_threshold_table.to_string(index=False, float_format=lambda v: f"{v:.4f}"))

print(
    f"\n🎯 Final deployed threshold: {best_threshold:.2f} "
    f"(precision={best_threshold_row['Precision']:.4f}, recall={best_threshold_row['Recall']:.4f}, "
    f"f1={best_threshold_row['F1']:.4f}, recall>=0.40={bool(best_threshold_row['RecallEligible'])})"
)

y_prob = deployed_model.predict_proba(X_test_scaled)[:, 1]
y_pred_default = (y_prob >= 0.5).astype(int)
y_pred_selected = (y_prob >= best_threshold).astype(int)

default_precision = precision_score(y_test, y_pred_default, zero_division=0)
default_recall = recall_score(y_test, y_pred_default, zero_division=0)
default_f1 = f1_score(y_test, y_pred_default, zero_division=0)

selected_precision = precision_score(y_test, y_pred_selected, zero_division=0)
selected_recall = recall_score(y_test, y_pred_selected, zero_division=0)
selected_f1 = f1_score(y_test, y_pred_selected, zero_division=0)

roc_auc = roc_auc_score(y_test, y_prob)
pr_auc = average_precision_score(y_test, y_prob)

print("\n📈 Threshold comparison:")
print(
    pd.DataFrame(
        [
            {
                "Threshold": 0.5,
                "Precision": default_precision,
                "Recall": default_recall,
                "F1": default_f1,
            },
            {
                "Threshold": best_threshold,
                "Precision": selected_precision,
                "Recall": selected_recall,
                "F1": selected_f1,
            },
        ]
    ).to_string(index=False, float_format=lambda v: f"{v:.4f}")
)

test_threshold_table = evaluate_threshold_grid(y_test, y_prob, threshold_grid)
print("\n📈 Threshold comparison table (test, 0.3 -> 0.9):")
print(test_threshold_table.to_string(index=False, float_format=lambda v: f"{v:.4f}"))

print("\n🧱 Confusion matrix (selected threshold):")
print(confusion_matrix(y_test, y_pred_selected))

print("\n📋 Classification report (selected threshold):")
print(classification_report(y_test, y_pred_selected, target_names=["No Stroke", "Stroke"], zero_division=0))

print(f"ROC-AUC: {roc_auc:.4f}")
print(f"PR-AUC: {pr_auc:.4f}")

curve_precision, curve_recall, curve_thresholds = precision_recall_curve(y_test, y_prob)
curve_df = pd.DataFrame(
    {
        "threshold": curve_thresholds,
        "precision": curve_precision[1:],
        "recall": curve_recall[1:],
    }
)

print("\n📉 Precision-Recall curve points (sample):")
if len(curve_df) > 12:
    sample_idx = np.linspace(0, len(curve_df) - 1, 12, dtype=int)
    curve_preview = curve_df.iloc[sample_idx]
else:
    curve_preview = curve_df
print(curve_preview.to_string(index=False, float_format=lambda v: f"{v:.4f}"))

joblib.dump(deployed_model, base_dir / "stroke_model.pkl")
joblib.dump(scaler, base_dir / "stroke_scaler.pkl")
joblib.dump(feature_names, base_dir / "stroke_features.pkl")
joblib.dump(
    {
        "threshold": best_threshold,
        "oof_threshold": oof_best_threshold,
        "best_model": best_model_name,
        "best_resampler": best_resampler_name,
        "calibrated": calibration_enabled,
        "calibration_method": calibration_method if calibration_enabled else None,
        "threshold_selection_mode": "precision_priority_with_recall_floor",
        "recall_floor": min_recall,
        "precision": selected_precision,
        "recall": selected_recall,
        "f1": selected_f1,
        "roc_auc": roc_auc,
        "pr_auc": pr_auc,
    },
    base_dir / "stroke_threshold_meta.pkl",
)

print("\n💾 Saved:")
print("   - stroke_model.pkl")
print("   - stroke_scaler.pkl")
print("   - stroke_features.pkl")
print("   - stroke_threshold_meta.pkl")
print("=" * 70)
print("✅ DONE")
print("=" * 70)
