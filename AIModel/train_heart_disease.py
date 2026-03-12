"""Train heart disease model with reusable imbalance-aware pipeline."""

from pathlib import Path
import warnings

import pandas as pd
from sklearn.ensemble import GradientBoostingClassifier, RandomForestClassifier
from sklearn.linear_model import LogisticRegression

from training_pipeline import train_binary_pipeline

warnings.filterwarnings("ignore")

print("=" * 70)
print("🚀 HEART DISEASE MODEL TRAINING")
print("=" * 70)

base_dir = Path(__file__).resolve().parent
data_path = base_dir / "data" / "heart.csv"
if not data_path.exists():
    raise FileNotFoundError(f"Không tìm thấy dataset: {data_path}")

print(f"\n📂 Loading dataset: {data_path}")
df = pd.read_csv(data_path)
print(f"✅ Rows: {len(df)}, Columns: {len(df.columns)}")

target_candidates = ["target", "HeartDisease", "output", "condition", "num"]
target_col = next((col for col in target_candidates if col in df.columns), None)
if target_col is None:
    raise ValueError(f"Không tìm thấy cột target. Columns hiện có: {list(df.columns)}")

before_dedup = len(df)
df = df.drop_duplicates().reset_index(drop=True)
after_dedup = len(df)
print(f"🧹 Deduplicate: {before_dedup} -> {after_dedup} rows (removed {before_dedup - after_dedup})")

X = df.drop(columns=[target_col]).copy()
y = df[target_col].astype(int)

for col in X.columns:
    if X[col].dtype == "object":
        X[col] = X[col].astype("category").cat.codes

models = {
    "LogisticRegression": LogisticRegression(max_iter=3000, random_state=42, class_weight="balanced"),
    "RandomForest": RandomForestClassifier(
        n_estimators=400,
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
        n_estimators=350,
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
    disease_name="heart disease",
    target_names=["No Disease", "Disease"],
    model_path=base_dir / "heart_disease_model.pkl",
    scaler_path=base_dir / "heart_disease_scaler.pkl",
    features_path=base_dir / "heart_disease_features.pkl",
    threshold_meta_path=base_dir / "heart_disease_threshold_meta.pkl",
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
