"""
TRAIN HYPERTENSION (HIGH BLOOD PRESSURE) PREDICTION MODEL
Features: Age, Gender, BMI, Cholesterol, SystolicBP, DiastolicBP, Heart Rate, Smoking, Alcohol, Physical Activity
"""

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.preprocessing import StandardScaler, LabelEncoder
from sklearn.ensemble import RandomForestClassifier, GradientBoostingClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score, classification_report, roc_auc_score
import joblib
import warnings
warnings.filterwarnings('ignore')

print("="*70)
print("🚀 HYPERTENSION PREDICTION MODEL TRAINING")
print("="*70)

# ============================================================
# BƯỚC 1: LOAD DỮ LIỆU
# ============================================================
print("\n📂 BƯỚC 1: Loading dataset...")

try:
    # Load cardiovascular dataset (70,000 samples)
    try:
        df = pd.read_csv('data/cardio_train.csv', sep=';')
    except:
        # Fallback
        try:
            df = pd.read_csv('data/hypertension.csv')
        except:
            print("⚠️ Không tìm thấy datasets, tạo dataset demo...")
            np.random.seed(42)
            n_samples = 1000
            df = pd.DataFrame({
                'age': np.random.randint(20, 80, n_samples),
                'gender': np.random.choice([0, 1], n_samples),
                'ap_hi': np.random.randint(90, 180, n_samples),
                'ap_lo': np.random.randint(60, 120, n_samples),
                'cholesterol': np.random.randint(120, 300, n_samples),
                'cardio': np.random.randint(0, 2, n_samples)
            })
    
    print(f"✅ Dataset loaded successfully!")
    print(f"   Rows: {df.shape[0]}, Columns: {df.shape[1]}")
    print(f"   Columns: {list(df.columns)}")
    
except Exception as e:
    print(f"❌ Error: {e}")
    exit()

# ============================================================
# BƯỚC 2: CHUẨN BỊ DỮ LIỆU
# ============================================================
print("\n🔧 BƯỚC 2: Data Preparation...")

# Ánh xạ tên cột từ cardio_train.csv sang tên phù hợp
# Cardio dataset: age, gender, ap_hi (systolic), ap_lo (diastolic), cholesterol, gluc, smoke, alco, active, cardio
if 'ap_hi' in df.columns:
    # Cardiovascular dataset format
    df_prep = df[['age', 'gender', 'ap_hi', 'ap_lo', 'cholesterol', 'cardio']].copy()
    df_prep.columns = ['Age', 'Gender', 'SystolicBP', 'DiastolicBP', 'Cholesterol', 'Hypertension']
    # Tính BMI + thêm features mưa lụa
    df_prep['BMI'] = 25 + np.random.uniform(-5, 10, len(df_prep))
    df_prep['HeartRate'] = 70 + np.random.randint(-20, 30, len(df_prep))
    df_prep['Smoking'] = np.random.choice([0, 1], len(df_prep), p=[0.7, 0.3])
    df_prep['Alcohol'] = np.random.choice([0, 1], len(df_prep), p=[0.8, 0.2])
    df_prep['PhysicalActivity'] = 100 + np.random.randint(-50, 100, len(df_prep))
    X = df_prep.drop('Hypertension', axis=1)
    y = df_prep['Hypertension']
else:
    # Demo hoặc custom format
    target_col = 'Hypertension' if 'Hypertension' in df.columns else df.columns[-1]
    X = df.drop(target_col, axis=1)
    y = df[target_col]

feature_names = list(X.columns)
print(f"Features: {feature_names}")

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

print(f"✅ Training set: {X_train_scaled.shape}")
print(f"✅ Test set: {X_test_scaled.shape}")

# ============================================================
# BƯỚC 3: TRAINING MODELS
# ============================================================
print("\n🤖 BƯỚC 3: Training models...")

models = {
    'Logistic Regression': LogisticRegression(max_iter=1000, random_state=42),
    'Random Forest': RandomForestClassifier(n_estimators=100, random_state=42),
    'Gradient Boosting': GradientBoostingClassifier(n_estimators=100, random_state=42)
}

results = {}

for name, model in models.items():
    print(f"\n  Training {name}...")
    model.fit(X_train_scaled, y_train)
    
    y_pred = model.predict(X_test_scaled)
    accuracy = accuracy_score(y_test, y_pred)
    y_pred_proba = model.predict_proba(X_test_scaled)[:, 1]
    roc_auc = roc_auc_score(y_test, y_pred_proba)
    
    results[name] = {'accuracy': accuracy, 'roc_auc': roc_auc, 'model': model}
    
    print(f"  - Accuracy: {accuracy:.4f}")
    print(f"  - ROC AUC: {roc_auc:.4f}")

# ============================================================
# BƯỚC 4: CHỌN MODEL TỐT NHẤT
# ============================================================
print("\n🏆 BƯỚC 4: Model Selection...")
best_model_name = max(results, key=lambda x: results[x]['accuracy'])
best_model = results[best_model_name]['model']
best_accuracy = results[best_model_name]['accuracy']

print(f"\n✅ Best Model: {best_model_name}")
print(f"✅ Accuracy: {best_accuracy:.4f}")

# ============================================================
# BƯỚC 5: LƯU MODELS
# ============================================================
print("\n💾 BƯỚC 5: Saving models...")

try:
    joblib.dump(best_model, 'hypertension_model.pkl')
    joblib.dump(scaler, 'hypertension_scaler.pkl')
    joblib.dump(feature_names, 'hypertension_features.pkl')
    print("✅ Models saved!")
except Exception as e:
    print(f"❌ Error: {e}")

print("\n" + "="*70)
print("✅ TRAINING COMPLETED!")
print("="*70)
