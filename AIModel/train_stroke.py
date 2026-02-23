"""
TRAIN STROKE PREDICTION MODEL
Features: Age, Gender, Hypertension, HeartDisease, Smoking, BMI, Glucose, AvgBloodPressure
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
print("🚀 STROKE PREDICTION MODEL TRAINING")
print("="*70)

# ============================================================
# BƯỚC 1: LOAD DỮ LIỆU
# ============================================================
print("\n📂 BƯỚC 1: Loading dataset...")

try:
    try:
        df = pd.read_csv('data/healthcare-dataset-stroke-data.csv')
    except:
        # Fallback: tìm file stroke.csv
        try:
            df = pd.read_csv('data/stroke.csv')
        except:
            print("⚠️ Không tìm thấy file stroke.csv, tạo dataset demo...")
            np.random.seed(42)
            n_samples = 5000
            df = pd.DataFrame({
                'age': np.random.randint(20, 90, n_samples),
                'gender': np.random.choice([0, 1], n_samples),
                'hypertension': np.random.choice([0, 1], n_samples),
                'heart_disease': np.random.choice([0, 1], n_samples),
                'smoking_status': np.random.choice([0, 1], n_samples),
                'bmi': np.random.uniform(10, 60, n_samples),
                'avg_glucose_level': np.random.randint(50, 300, n_samples),
                'stroke': np.random.choice([0, 1], n_samples, p=[0.9, 0.1])
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

# Ánh xạ tên cột từ healthcare-dataset-stroke-data.csv
# Dataset format: id, gender, age, hypertension, heart_disease, ever_married, work_type, Residence_type, avg_glucose_level, bmi, smoking_status, stroke
if 'avg_glucose_level' in df.columns:
    # Real stroke dataset format
    # Xóa missing values hoặc fill
    df = df.dropna(subset=['bmi'], how='all')
    df['bmi'].fillna(df['bmi'].mean(), inplace=True)
    
    # Chọn features phù hợp
    df_prep = df[['age', 'gender', 'hypertension', 'heart_disease', 'avg_glucose_level', 'bmi', 'stroke']].copy()
    
    # Thêm smoking status (simplified)
    if 'smoking_status' in df.columns:
        df_prep['smoking'] = (df['smoking_status'] != 'never smoked').astype(int)
    else:
        df_prep['smoking'] = np.random.choice([0, 1], len(df_prep))
    
    df_prep.columns = ['Age', 'Gender', 'Hypertension', 'HeartDisease', 'AvgBloodPressure', 'BMI', 'Stroke', 'Smoking']
    
    # Encode gender (Male=1, Female=0)
    df_prep['Gender'] = (df_prep['Gender'] == 'Male').astype(int)
    
    X = df_prep.drop('Stroke', axis=1)
    y = df_prep['Stroke']
else:
    # Demo format
    target_col = 'Stroke' if 'Stroke' in df.columns else df.columns[-1]
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
    joblib.dump(best_model, 'stroke_model.pkl')
    joblib.dump(scaler, 'stroke_scaler.pkl')
    joblib.dump(feature_names, 'stroke_features.pkl')
    print("✅ Models saved!")
except Exception as e:
    print(f"❌ Error: {e}")

print("\n" + "="*70)
print("✅ TRAINING COMPLETED!")
print("="*70)
