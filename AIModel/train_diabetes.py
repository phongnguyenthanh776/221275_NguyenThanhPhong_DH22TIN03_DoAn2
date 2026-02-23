"""
TRAIN DIABETES PREDICTION MODEL
Dataset: Diabetes Classification từ Kaggle
Features: Pregnancies, Glucose, BloodPressure, SkinThickness, Insulin, BMI, DiabetesPedigreeFunction, Age
"""

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.preprocessing import StandardScaler
from sklearn.ensemble import RandomForestClassifier, GradientBoostingClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score, classification_report, roc_auc_score
import joblib
import warnings
warnings.filterwarnings('ignore')

print("="*70)
print("🚀 DIABETES PREDICTION MODEL TRAINING")
print("="*70)

# ============================================================
# BƯỚC 1: LOAD DỮ LIỆU
# ============================================================
print("\n📂 BƯỚC 1: Loading dataset...")

try:
    # Thử đọc từ nhiều tên file có thể có
    try:
        df = pd.read_csv('data/diabetes.csv')
    except:
        try:
            df = pd.read_csv('data/pima-indians-diabetes.csv')
        except:
            # Nếu không có file, tạo dataset mẫu
            print("⚠️ Không tìm thấy file diabetes.csv, tạo dataset demo...")
            np.random.seed(42)
            n_samples = 768
            df = pd.DataFrame({
                'Pregnancies': np.random.randint(0, 15, n_samples),
                'Glucose': np.random.randint(44, 200, n_samples),
                'BloodPressure': np.random.randint(24, 122, n_samples),
                'SkinThickness': np.random.randint(0, 100, n_samples),
                'Insulin': np.random.randint(0, 846, n_samples),
                'BMI': np.random.uniform(18, 70, n_samples),
                'DiabetesPedigreeFunction': np.random.uniform(0, 2.5, n_samples),
                'Age': np.random.randint(21, 81, n_samples),
                'Outcome': np.random.randint(0, 2, n_samples)
            })
    
    print(f"✅ Dataset loaded successfully!")
    print(f"   Rows: {df.shape[0]}, Columns: {df.shape[1]}")
    print(f"\n📋 Columns: {list(df.columns)}")
    
except Exception as e:
    print(f"❌ Error loading dataset: {e}")
    print("\n📝 Guide:")
    print("   Download from: https://www.kaggle.com/datasets/uciml/pima-indians-diabetes-database")
    exit()

# ============================================================
# BƯỚC 2: KIỂM TRA DỮ LIỆU
# ============================================================
print("\n📊 BƯỚC 2: Data Exploration...")
print(f"Missing values:\n{df.isnull().sum()}")
print(f"\nData types:\n{df.dtypes}")
print(f"\nTarget distribution:\n{df['Outcome'].value_counts()}")

# ============================================================
# BƯỚC 3: CHUẨN BỊ DỮ LIỆU
# ============================================================
print("\n🔧 BƯỚC 3: Data Preparation...")

# Xác định features và target
X = df.drop('Outcome', axis=1)
y = df['Outcome']

# Lưu feature names
feature_names = list(X.columns)
print(f"Features: {feature_names}")

# Chia train/test
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

# Chuẩn hóa dữ liệu
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

print(f"✅ Training set: {X_train_scaled.shape}")
print(f"✅ Test set: {X_test_scaled.shape}")

# ============================================================
# BƯỚC 4: TRAINING MODELS
# ============================================================
print("\n🤖 BƯỚC 4: Training models...")

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
# BƯỚC 5: CHỌN MODEL TỐT NHẤT
# ============================================================
print("\n🏆 BƯỚC 5: Model Selection...")
best_model_name = max(results, key=lambda x: results[x]['accuracy'])
best_model = results[best_model_name]['model']
best_accuracy = results[best_model_name]['accuracy']

print(f"\n✅ Best Model: {best_model_name}")
print(f"✅ Accuracy: {best_accuracy:.4f}")

# ============================================================
# BƯỚC 6: LƯỚI MODELS
# ============================================================
print("\n💾 BƯỚC 6: Saving models...")

try:
    joblib.dump(best_model, 'diabetes_model.pkl')
    joblib.dump(scaler, 'diabetes_scaler.pkl')
    joblib.dump(feature_names, 'diabetes_features.pkl')
    print("✅ Models saved successfully!")
    print("   - diabetes_model.pkl")
    print("   - diabetes_scaler.pkl")
    print("   - diabetes_features.pkl")
except Exception as e:
    print(f"❌ Error saving models: {e}")

# ============================================================
# BƯỚC 7: DETAILED REPORT
# ============================================================
print("\n📈 BƯỚC 7: Detailed Report...")
print(f"\nBest Model: {best_model_name}")
print(f"Accuracy: {best_accuracy:.4f}")

y_pred = best_model.predict(X_test_scaled)
print(f"\n{classification_report(y_test, y_pred, target_names=['No Diabetes', 'Diabetes'])}")

print("\n" + "="*70)
print("✅ TRAINING COMPLETED!")
print("="*70)
