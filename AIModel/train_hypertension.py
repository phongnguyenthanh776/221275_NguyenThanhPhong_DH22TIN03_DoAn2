"""Train hypertension/cardio model with reusable imbalance-aware pipeline."""

from pathlib import Path
import warnings

import pandas as pd
from sklearn.ensemble import GradientBoostingClassifier, RandomForestClassifier
from sklearn.linear_model import LogisticRegression

from training_pipeline import train_binary_pipeline

warnings.filterwarnings("ignore")

print("=" * 70)
print("🚀 HYPERTENSION/CARDIO MODEL TRAINING")
print("=" * 70)

base_dir = Path(__file__).resolve().parent
data_path = base_dir / "data" / "cardio_train.csv"
if not data_path.exists():
    raise FileNotFoundError(f"Không tìm thấy dataset: {data_path}")

print(f"\n📂 Loading dataset: {data_path}")
df = pd.read_csv(data_path, sep=";")
print(f"✅ Rows: {len(df)}, Columns: {len(df.columns)}")

required_columns = ["age", "gender", "height", "weight", "ap_hi", "ap_lo", "cholesterol", "smoke", "alco", "active", "cardio"]
missing_columns = [col for col in required_columns if col not in df.columns]
if missing_columns:
    raise ValueError(f"Thiếu cột bắt buộc: {missing_columns}")

df = df[(df["ap_hi"] >= 80) & (df["ap_hi"] <= 240)]
df = df[(df["ap_lo"] >= 40) & (df["ap_lo"] <= 160)]
df = df[df["ap_hi"] > df["ap_lo"]]

prep = pd.DataFrame()
prep["Age"] = (df["age"] / 365.25).round(1)
prep["Gender"] = (df["gender"] == 2).astype(int)
prep["SystolicBP"] = df["ap_hi"]
prep["DiastolicBP"] = df["ap_lo"]
prep["Cholesterol"] = df["cholesterol"]
prep["BMI"] = (df["weight"] / ((df["height"] / 100) ** 2)).clip(lower=12, upper=55)
prep["Smoking"] = df["smoke"].astype(int)
prep["Alcohol"] = df["alco"].astype(int)
prep["PhysicalActivity"] = df["active"].astype(int)

X = prep
y = df["cardio"].astype(int)

models = {
    "LogisticRegression": LogisticRegression(max_iter=3000, random_state=42, class_weight="balanced"),
    "RandomForest": RandomForestClassifier(
        n_estimators=500,
        min_samples_leaf=3,
        random_state=42,
        class_weight="balanced",
        n_jobs=-1,
    ),
    "GradientBoosting": GradientBoostingClassifier(random_state=42),
}

try:
    from xgboost import XGBClassifier

    models["XGBoost"] = XGBClassifier(
        n_estimators=320,
        learning_rate=0.05,
        max_depth=4,
        subsample=0.9,
        colsample_bytree=0.9,
        reg_lambda=1.0,
        objective="binary:logistic",
        eval_metric="logloss",
        random_state=42,
        n_jobs=-1,
        tree_method="hist",
    )
    print("✅ XGBoost available: included in model comparison")
except Exception:
    print("⚠️ XGBoost not available: skipped")

try:
    from lightgbm import LGBMClassifier

    models["LightGBM"] = LGBMClassifier(
        n_estimators=450,
        learning_rate=0.05,
        num_leaves=31,
        subsample=0.9,
        colsample_bytree=0.9,
        random_state=42,
    )
    print("✅ LightGBM available: included in model comparison")
except Exception:
    print("⚠️ LightGBM not available: skipped")

train_binary_pipeline(
    X=X,
    y=y,
    models=models,
    disease_name="hypertension",
    target_names=["Low Risk", "High Risk"],
    model_path=base_dir / "hypertension_model.pkl",
    scaler_path=base_dir / "hypertension_scaler.pkl",
    features_path=base_dir / "hypertension_features.pkl",
    threshold_meta_path=base_dir / "hypertension_threshold_meta.pkl",
    imbalance_ratio_threshold=0.85,
    calibrate_probabilities=True,
    calibration_method='sigmoid',
    calibration_size=0.2,
    threshold_metric='f1',
    min_recall_for_threshold=0.55,
)

print("=" * 70)
print("✅ DONE")
print("=" * 70)
