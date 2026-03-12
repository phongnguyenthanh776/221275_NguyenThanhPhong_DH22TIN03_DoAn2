from pathlib import Path
from typing import Callable, Optional

import joblib
import numpy as np
import pandas as pd
from imblearn.over_sampling import SMOTE
from sklearn.calibration import CalibratedClassifierCV
from sklearn.base import clone
from sklearn.metrics import (
    accuracy_score,
    average_precision_score,
    classification_report,
    confusion_matrix,
    f1_score,
    precision_score,
    recall_score,
    roc_auc_score,
)
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler


def _print_distribution(y: pd.Series, title: str) -> pd.Series:
    counts = y.value_counts().sort_index()
    ratio = (counts / len(y) * 100).round(2)
    distribution_df = pd.DataFrame({"count": counts, "ratio_%": ratio})
    print(f"\n📊 Class distribution - {title}:")
    print(distribution_df.to_string())
    return counts


def _imbalance_ratio(counts: pd.Series) -> float:
    if len(counts) < 2:
        return 1.0
    return float(counts.min() / counts.max())


def _safe_roc_auc(y_true: pd.Series, y_prob: np.ndarray) -> float:
    try:
        return float(roc_auc_score(y_true, y_prob))
    except ValueError:
        return float('nan')


def _safe_pr_auc(y_true: pd.Series, y_prob: np.ndarray) -> float:
    try:
        return float(average_precision_score(y_true, y_prob))
    except ValueError:
        return float('nan')


def _build_calibrator(model, method: str = 'sigmoid'):
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


def _pick_best_threshold(
    *,
    y_true: pd.Series,
    y_prob: np.ndarray,
    threshold_grid: np.ndarray,
    metric: str,
    min_recall: Optional[float],
) -> tuple:
    rows = []
    for threshold in threshold_grid:
        y_pred = (y_prob >= threshold).astype(int)
        rows.append(
            {
                'Threshold': float(threshold),
                'Precision': precision_score(y_true, y_pred, zero_division=0),
                'Recall': recall_score(y_true, y_pred, zero_division=0),
                'F1': f1_score(y_true, y_pred, zero_division=0),
            }
        )

    threshold_df = pd.DataFrame(rows)
    if threshold_df.empty:
        return 0.5, None, threshold_df

    metric_col = str(metric or 'f1').strip().upper()
    if metric_col not in {'F1', 'PRECISION', 'RECALL'}:
        metric_col = 'F1'

    threshold_df['RecallEligible'] = True
    candidate_df = threshold_df
    if min_recall is not None:
        threshold_df['RecallEligible'] = threshold_df['Recall'] >= float(min_recall)
        eligible = threshold_df[threshold_df['RecallEligible']]
        if not eligible.empty:
            candidate_df = eligible

    ordered = candidate_df.sort_values(
        by=[metric_col, 'Precision', 'Recall', 'Threshold'],
        ascending=[False, False, False, True],
    )
    best_row = ordered.iloc[0]
    return float(best_row['Threshold']), best_row, threshold_df


def train_binary_pipeline(
    *,
    X: pd.DataFrame,
    y: pd.Series,
    models: dict,
    disease_name: str,
    target_names: list,
    model_path: Path,
    scaler_path: Path,
    features_path: Path,
    ranking_fn: Optional[Callable[[dict], float]] = None,
    imbalance_ratio_threshold: float = 0.85,
    force_smote: bool = False,
    threshold_meta_path: Optional[Path] = None,
    calibrate_probabilities: bool = True,
    calibration_method: str = 'sigmoid',
    calibration_size: float = 0.2,
    threshold_metric: str = 'f1',
    min_recall_for_threshold: Optional[float] = None,
    threshold_grid: Optional[np.ndarray] = None,
    test_size: float = 0.2,
    random_state: int = 42,
) -> dict:
    X = X.copy().reset_index(drop=True)
    y = pd.Series(y).astype(int).reset_index(drop=True)
    feature_names = list(X.columns)

    full_counts = _print_distribution(y, "full dataset")
    full_ratio = _imbalance_ratio(full_counts)
    print(f"ℹ️ Imbalance ratio (minority/majority): {full_ratio:.4f}")

    X_train_full, X_test, y_train_full, y_test = train_test_split(
        X,
        y,
        test_size=test_size,
        random_state=random_state,
        stratify=y,
    )

    X_train = X_train_full
    y_train = y_train_full
    X_cal = None
    y_cal = None

    calibration_size = float(calibration_size)
    calibration_size = min(max(calibration_size, 0.0), 0.4)
    if calibrate_probabilities and calibration_size > 0:
        try:
            X_train, X_cal, y_train, y_cal = train_test_split(
                X_train_full,
                y_train_full,
                test_size=calibration_size,
                random_state=random_state,
                stratify=y_train_full,
            )
            print(
                f"🧪 Calibration split prepared: train={len(X_train)}, calibration={len(X_cal)}, test={len(X_test)}"
            )
        except Exception as split_err:
            print(f"⚠️ Calibration split failed, fallback to uncalibrated model: {split_err}")
            X_cal = None
            y_cal = None

    train_counts = _print_distribution(y_train, "train split (before SMOTE)")
    train_ratio = _imbalance_ratio(train_counts)
    use_smote = force_smote or (train_ratio < imbalance_ratio_threshold)

    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)
    X_cal_scaled = scaler.transform(X_cal) if X_cal is not None else None

    if use_smote:
        print(
            f"\n⚖️ SMOTE enabled for {disease_name} "
            f"(train ratio={train_ratio:.4f}, threshold={imbalance_ratio_threshold:.2f})"
        )
        smote = SMOTE(random_state=random_state)
        X_train_used, y_train_used = smote.fit_resample(X_train_scaled, y_train)
        _print_distribution(pd.Series(y_train_used), "train split (after SMOTE)")
    else:
        print(
            f"\n✅ SMOTE skipped for {disease_name} "
            f"(train ratio={train_ratio:.4f} >= threshold={imbalance_ratio_threshold:.2f})"
        )
        X_train_used, y_train_used = X_train_scaled, y_train

    results = []
    best_model = None
    best_name = None
    best_score = -1.0

    print("\n🤖 Training models...")
    for name, model in models.items():
        model_instance = clone(model)
        model_instance.fit(X_train_used, y_train_used)

        y_pred = model_instance.predict(X_test_scaled)
        if hasattr(model_instance, 'predict_proba'):
            y_prob = model_instance.predict_proba(X_test_scaled)[:, 1]
        else:
            y_prob = y_pred.astype(float)

        accuracy = accuracy_score(y_test, y_pred)
        precision = precision_score(y_test, y_pred, zero_division=0)
        recall = recall_score(y_test, y_pred, zero_division=0)
        f1 = f1_score(y_test, y_pred, zero_division=0)

        roc_auc = _safe_roc_auc(y_test, y_prob)
        pr_auc = _safe_pr_auc(y_test, y_prob)

        metrics = {
            "Model": name,
            "Accuracy": accuracy,
            "Precision": precision,
            "Recall": recall,
            "F1": f1,
            "ROC_AUC": roc_auc,
            "PR_AUC": pr_auc,
        }

        if ranking_fn is not None:
            ranking_score = ranking_fn(metrics)
        else:
            if not np.isnan(roc_auc) and not np.isnan(pr_auc):
                ranking_score = (0.50 * roc_auc) + (0.30 * pr_auc) + (0.20 * f1)
            elif not np.isnan(roc_auc):
                ranking_score = (0.70 * roc_auc) + (0.30 * f1)
            else:
                ranking_score = f1

        metrics["RankingScore"] = ranking_score
        results.append(metrics)

        if ranking_score > best_score:
            best_score = ranking_score
            best_name = name
            best_model = model_instance

    result_df = pd.DataFrame(results).sort_values(by="RankingScore", ascending=False)
    print("\n📊 Model comparison:")
    print(result_df.to_string(index=False, float_format=lambda x: f"{x:.4f}"))

    print(f"\n🏆 Best model (uncalibrated): {best_name}")

    deployed_model = best_model
    is_calibrated = False
    threshold_table = pd.DataFrame()
    decision_threshold = 0.5

    can_calibrate = (
        calibrate_probabilities
        and X_cal_scaled is not None
        and y_cal is not None
        and len(pd.Series(y_cal).unique()) > 1
        and hasattr(best_model, 'predict_proba')
    )

    if can_calibrate:
        try:
            calibrator = _build_calibrator(best_model, method=calibration_method)
            calibrator.fit(X_cal_scaled, y_cal)
            deployed_model = calibrator
            is_calibrated = True
            print(f"✅ Probability calibration enabled ({calibration_method})")
        except Exception as calibrate_err:
            print(f"⚠️ Calibration failed. Use uncalibrated model: {calibrate_err}")
    elif calibrate_probabilities:
        print("⚠️ Calibration skipped (not enough valid calibration data or model lacks predict_proba)")

    if threshold_grid is None:
        threshold_grid = np.round(
            np.concatenate([
                np.arange(0.10, 0.31, 0.05),
                np.arange(0.35, 0.901, 0.05),
            ]),
            2,
        )

    if hasattr(deployed_model, 'predict_proba') and X_cal_scaled is not None and y_cal is not None:
        calibration_prob = deployed_model.predict_proba(X_cal_scaled)[:, 1]
        decision_threshold, best_threshold_row, threshold_table = _pick_best_threshold(
            y_true=y_cal,
            y_prob=calibration_prob,
            threshold_grid=np.asarray(threshold_grid, dtype=np.float32),
            metric=threshold_metric,
            min_recall=min_recall_for_threshold,
        )
        if best_threshold_row is not None:
            print(
                "🎯 Selected threshold from calibration split: "
                f"{decision_threshold:.2f} "
                f"(precision={best_threshold_row['Precision']:.4f}, "
                f"recall={best_threshold_row['Recall']:.4f}, f1={best_threshold_row['F1']:.4f})"
            )
    else:
        print("ℹ️ Using default threshold 0.50 (no calibration split available for threshold tuning)")

    if hasattr(deployed_model, 'predict_proba'):
        test_prob = deployed_model.predict_proba(X_test_scaled)[:, 1]
        best_pred = (test_prob >= decision_threshold).astype(int)
        test_roc_auc = _safe_roc_auc(y_test, test_prob)
        test_pr_auc = _safe_pr_auc(y_test, test_prob)
    else:
        best_pred = deployed_model.predict(X_test_scaled)
        test_prob = best_pred.astype(float)
        test_roc_auc = float('nan')
        test_pr_auc = float('nan')

    cm = confusion_matrix(y_test, best_pred)
    test_f1 = f1_score(y_test, best_pred, zero_division=0)
    test_precision = precision_score(y_test, best_pred, zero_division=0)
    test_recall = recall_score(y_test, best_pred, zero_division=0)
    test_accuracy = accuracy_score(y_test, best_pred)

    print("\n🧱 Confusion matrix (best model):")
    print(cm)

    print("\n📋 Classification report (best model):")
    print(classification_report(y_test, best_pred, target_names=target_names, zero_division=0))

    print("\n📈 Final metrics (deployed model):")
    metric_rows = [
        {
            'Accuracy': test_accuracy,
            'Precision': test_precision,
            'Recall': test_recall,
            'F1': test_f1,
            'ROC_AUC': test_roc_auc,
            'PR_AUC': test_pr_auc,
            'DecisionThreshold': decision_threshold,
            'Calibrated': is_calibrated,
        }
    ]
    print(pd.DataFrame(metric_rows).to_string(index=False, float_format=lambda x: f"{x:.4f}"))

    if not threshold_table.empty:
        print("\n📉 Threshold table (calibration split):")
        print(threshold_table.to_string(index=False, float_format=lambda x: f"{x:.4f}"))

    joblib.dump(deployed_model, model_path)
    joblib.dump(scaler, scaler_path)
    joblib.dump(feature_names, features_path)

    saved_threshold_meta_path = None
    if threshold_meta_path is not None:
        threshold_meta = {
            'threshold': float(decision_threshold),
            'best_model': str(best_name),
            'calibrated': bool(is_calibrated),
            'calibration_method': calibration_method if is_calibrated else None,
            'threshold_metric': str(threshold_metric).lower(),
            'min_recall_for_threshold': float(min_recall_for_threshold) if min_recall_for_threshold is not None else None,
            'roc_auc': None if np.isnan(test_roc_auc) else float(test_roc_auc),
            'pr_auc': None if np.isnan(test_pr_auc) else float(test_pr_auc),
            'f1': float(test_f1),
            'precision': float(test_precision),
            'recall': float(test_recall),
        }
        joblib.dump(threshold_meta, threshold_meta_path)
        saved_threshold_meta_path = threshold_meta_path

    print("\n💾 Saved:")
    print(f"   - {model_path.name}")
    print(f"   - {scaler_path.name}")
    print(f"   - {features_path.name}")
    if saved_threshold_meta_path is not None:
        print(f"   - {saved_threshold_meta_path.name}")

    return {
        "best_model": deployed_model,
        "best_model_name": best_name,
        "results": result_df,
        "confusion_matrix": cm,
        "decision_threshold": float(decision_threshold),
        "calibrated": bool(is_calibrated),
        "threshold_table": threshold_table,
    }