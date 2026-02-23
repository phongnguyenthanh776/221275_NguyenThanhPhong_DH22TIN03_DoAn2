
"""
TRAIN HEART DISEASE PREDICTION MODEL (SIMPLE)
Dataset: heart.csv (Kaggle)
"""


import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, classification_report
import joblib
import os

print("==============================")
print("TRAIN HEART DISEASE MODEL")
print("==============================")

# Đường dẫn file csv
csv_path = os.path.join(os.path.dirname(__file__), "data", "heart.csv")
print(f"[DEBUG] Đường dẫn tuyệt đối file: {csv_path}")
if not os.path.exists(csv_path):
    raise FileNotFoundError(f"Không tìm thấy file: {csv_path}")

# Đọc dữ liệu
print("Đang đọc dữ liệu...")
df = pd.read_csv(csv_path)
print(f"Đã load dữ liệu: {df.shape[0]} dòng, {df.shape[1]} cột")

# Xác định cột target duy nhất
possible_targets = ["target", "HeartDisease", "output", "condition", "num"]
target_col = None
for col in possible_targets:
    if col in df.columns:
        target_col = col
        break
if not target_col:
    print("❌ Không tìm thấy cột target trong dữ liệu!")
    print(f"Các cột có: {list(df.columns)}")
    exit(1)
print(f"✅ Cột target: {target_col}")

# Tiền xử lý dữ liệu
X = df.drop(target_col, axis=1)
y = df[target_col]

# Encode categorical columns nếu có
for col in X.columns:
    if X[col].dtype == 'object':
        X[col] = X[col].astype('category').cat.codes

# Chia train/test
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

# Scale features
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

# Train model
model = RandomForestClassifier(n_estimators=100, random_state=42)
model.fit(X_train_scaled, y_train)

# Đánh giá
y_pred = model.predict(X_test_scaled)
acc = accuracy_score(y_test, y_pred)
print(f"Độ chính xác trên tập test: {acc*100:.2f}%")
print(classification_report(y_test, y_pred))

# Lưu model, scaler, features
joblib.dump(model, "heart_model.pkl")
joblib.dump(scaler, "heart_scaler.pkl")
joblib.dump(list(X.columns), "heart_features.pkl")
print("Đã lưu model, scaler, features!")

# 2. Xác định cột target
possible_targets = ["target", "HeartDisease", "output", "condition", "num"]
target_col = None
for col in possible_targets:
    if col in df.columns:
        target_col = col
        break
if not target_col:
    print("❌ Không tìm thấy cột target trong dữ liệu!")
    print(f"Các cột có: {list(df.columns)}")
    exit(1)
print(f"✅ Cột target: {target_col}")

# 3. Tiền xử lý dữ liệu
X = df.drop(target_col, axis=1)
y = df[target_col]

# Encode categorical columns nếu có
for col in X.columns:
    if X[col].dtype == 'object':
        X[col] = X[col].astype('category').cat.codes

# 4. Chia train/test
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

# 5. Scale features
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

# 6. Train model
model = RandomForestClassifier(n_estimators=100, random_state=42)
model.fit(X_train_scaled, y_train)

# 7. Đánh giá
y_pred = model.predict(X_test_scaled)
acc = accuracy_score(y_test, y_pred)
print(f"✅ Độ chính xác trên tập test: {acc*100:.2f}%")
print(classification_report(y_test, y_pred))

# 8. Lưu model, scaler, features

# Lưu model với tên đồng bộ
joblib.dump(model, "heart_model.pkl")
joblib.dump(scaler, "heart_scaler.pkl")
joblib.dump(list(X.columns), "heart_features.pkl")
print("✅ Đã lưu model, scaler, features!")

print("\nHoàn thành train mô hình bệnh tim!")

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.preprocessing import StandardScaler, LabelEncoder
from sklearn.linear_model import LogisticRegression
from sklearn.ensemble import RandomForestClassifier, GradientBoostingClassifier
from sklearn.svm import SVC
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix, roc_auc_score
import joblib
import matplotlib.pyplot as plt
import seaborn as sns
import warnings
warnings.filterwarnings('ignore')


target_columns = ['HeartDisease', 'target', 'output', 'condition', 'num']
target_col = None
for col in target_columns:
    if col in df.columns:
        target_col = col
        break

if target_col is None:
    print("❌ Cannot find target column!")
    print(f"   Available columns: {list(df.columns)}")
    exit()

print(f"✅ Target column: {target_col}")

# Tách X và y
X = df.drop(target_col, axis=1)
y = df[target_col]

# Encode categorical features nếu có
le = LabelEncoder()
for col in X.columns:
    if X[col].dtype == 'object':
        print(f"   Encoding column: {col}")
        X[col] = le.fit_transform(X[col])

print(f"\n✅ Features: {list(X.columns)}")
print(f"✅ Target distribution:")
print(y.value_counts())
print(f"   Class 0: {(y == 0).sum()} ({(y == 0).sum()/len(y)*100:.1f}%)")
print(f"   Class 1: {(y == 1).sum()} ({(y == 1).sum()/len(y)*100:.1f}%)")

# ============================================================
# BƯỚC 4: SPLIT DATA
# ============================================================
print("\n✂️  BƯỚC 4: Splitting data...")
X_train, X_test, y_train, y_test = train_test_split(
    X, y, test_size=0.2, random_state=42, stratify=y
)
print(f"✅ Train set: {X_train.shape[0]} samples")
print(f"✅ Test set: {X_test.shape[0]} samples")

# ============================================================
# BƯỚC 5: FEATURE SCALING
# ============================================================
print("\n📏 BƯỚC 5: Feature Scaling...")
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)
print("✅ Scaling completed!")

# ============================================================
# BƯỚC 6: TRAIN MULTIPLE MODELS
# ============================================================
print("\n🤖 BƯỚC 6: Training Multiple Models...")
print("-"*70)

models = {
    'Logistic Regression': LogisticRegression(max_iter=1000, random_state=42),
    'Random Forest': RandomForestClassifier(n_estimators=100, random_state=42),
    'Gradient Boosting': GradientBoostingClassifier(n_estimators=100, random_state=42),
    'SVM': SVC(kernel='rbf', probability=True, random_state=42)
}

best_model = None
best_model_name = None
best_accuracy = 0
results = {}

for name, model in models.items():
    print(f"\n🔹 Training {name}...")
    
    # Train model
    model.fit(X_train_scaled, y_train)
    
    # Predict
    y_pred = model.predict(X_test_scaled)
    y_pred_proba = model.predict_proba(X_test_scaled)[:, 1] if hasattr(model, 'predict_proba') else None
    
    # Evaluate
    accuracy = accuracy_score(y_test, y_pred)
    
    # Cross-validation
    cv_scores = cross_val_score(model, X_train_scaled, y_train, cv=5)
    
    print(f"   ✅ Accuracy: {accuracy:.4f} ({accuracy*100:.2f}%)")
    print(f"   ✅ Cross-val score: {cv_scores.mean():.4f} ± {cv_scores.std():.4f}")
    
    if y_pred_proba is not None:
        roc_auc = roc_auc_score(y_test, y_pred_proba)
        print(f"   ✅ ROC-AUC: {roc_auc:.4f}")
    
    print(f"\n   📊 Classification Report:")
    print("   " + "-"*65)
    report = classification_report(y_test, y_pred, target_names=['No Disease', 'Disease'])
    for line in report.split('\n'):
        print(f"   {line}")
    
    # Save results
    results[name] = {
        'accuracy': accuracy,
        'cv_score': cv_scores.mean(),
        'model': model
    }
    
    # Track best model
    if accuracy > best_accuracy:
        best_accuracy = accuracy
        best_model = model
        best_model_name = name

# ============================================================
# BƯỚC 7: SAVE BEST MODEL
# ============================================================
print("\n"+"="*70)
print("🏆 BEST MODEL RESULTS")
print("="*70)
print(f"✅ Best Model: {best_model_name}")
print(f"✅ Accuracy: {best_accuracy:.4f} ({best_accuracy*100:.2f}%)")

print("\n💾 Saving models...")
joblib.dump(best_model, 'heart_disease_model.pkl')
joblib.dump(scaler, 'heart_disease_scaler.pkl')
print("✅ Saved: heart_disease_model.pkl")
print("✅ Saved: heart_disease_scaler.pkl")

# Lưu feature names
feature_names = list(X.columns)
joblib.dump(feature_names, 'heart_disease_features.pkl')
print(f"✅ Saved: heart_disease_features.pkl")
print(f"   Features: {feature_names}")

# ============================================================
# BƯỚC 8: VISUALIZATION
# ============================================================
print("\n📊 BƯỚC 8: Creating visualizations...")

# 1. Model Comparison
plt.figure(figsize=(10, 6))
model_names = list(results.keys())
accuracies = [results[m]['accuracy'] for m in model_names]
plt.bar(model_names, accuracies, color=['#3498db', '#2ecc71', '#e74c3c', '#f39c12'])
plt.ylim(0.5, 1.0)
plt.ylabel('Accuracy')
plt.title('Model Comparison - Accuracy')
plt.xticks(rotation=45, ha='right')
for i, v in enumerate(accuracies):
    plt.text(i, v + 0.01, f'{v:.3f}', ha='center', va='bottom', fontweight='bold')
plt.tight_layout()
plt.savefig('model_comparison.png', dpi=300, bbox_inches='tight')
print("✅ Saved: model_comparison.png")

# 2. Confusion Matrix
y_pred_best = best_model.predict(X_test_scaled)
cm = confusion_matrix(y_test, y_pred_best)
plt.figure(figsize=(8, 6))
sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', 
            xticklabels=['No Disease', 'Disease'],
            yticklabels=['No Disease', 'Disease'])
plt.title(f'Confusion Matrix - {best_model_name}')
plt.ylabel('Actual')
plt.xlabel('Predicted')
plt.tight_layout()
plt.savefig('confusion_matrix.png', dpi=300, bbox_inches='tight')
print("✅ Saved: confusion_matrix.png")

# 3. Feature Importance (if available)
if hasattr(best_model, 'feature_importances_'):
    importances = best_model.feature_importances_
    indices = np.argsort(importances)[::-1]
    
    plt.figure(figsize=(10, 6))
    plt.bar(range(len(importances)), importances[indices])
    plt.xticks(range(len(importances)), [feature_names[i] for i in indices], rotation=45, ha='right')
    plt.title(f'Feature Importances - {best_model_name}')
    plt.ylabel('Importance')
    plt.tight_layout()
    plt.savefig('feature_importance.png', dpi=300, bbox_inches='tight')
    print("✅ Saved: feature_importance.png")

# ============================================================
# BƯỚC 9: TEST PREDICTION
# ============================================================
print("\n🧪 BƯỚC 9: Testing prediction with sample data...")

# Sample input (khớp với form trong HealthManagement)
sample = {
    'Age': 55,
    'Sex': 1,  # Male
    'ChestPainType': 2,
    'RestingBP': 140,
    'Cholesterol': 250,
    'FastingBS': 1,
    'MaxHR': 150,
    'ExerciseAngina': 0
}

print(f"\n📝 Sample Input:")
for key, value in sample.items():
    print(f"   {key}: {value}")

# Convert to DataFrame with correct feature order
sample_df = pd.DataFrame([sample])

# Đảm bảo columns khớp với training data
for col in feature_names:
    if col not in sample_df.columns:
        sample_df[col] = 0  # Default value

sample_df = sample_df[feature_names]

# Predict
sample_scaled = scaler.transform(sample_df)
prediction = best_model.predict(sample_scaled)[0]
probability = best_model.predict_proba(sample_scaled)[0] if hasattr(best_model, 'predict_proba') else None

print(f"\n🎯 Prediction Result:")
print(f"   Class: {prediction} ({'Disease' if prediction == 1 else 'No Disease'})")
if probability is not None:
    print(f"   Probability: {probability[1]*100:.2f}% risk")
    if probability[1] >= 0.7:
        print(f"   Risk Level: HIGH ⚠️")
    elif probability[1] >= 0.4:
        print(f"   Risk Level: MEDIUM ⚡")
    else:
        print(f"   Risk Level: LOW ✅")

# ============================================================
# HOÀN THÀNH
# ============================================================
print("\n"+"="*70)
print("✅ TRAINING COMPLETED SUCCESSFULLY!")
print("="*70)
print("\n📁 Generated Files:")
print("   1. heart_disease_model.pkl      - Trained model")
print("   2. heart_disease_scaler.pkl     - Feature scaler")
print("   3. heart_disease_features.pkl   - Feature names")
print("   4. model_comparison.png         - Accuracy comparison chart")
print("   5. confusion_matrix.png         - Confusion matrix")
if hasattr(best_model, 'feature_importances_'):
    print("   6. feature_importance.png       - Feature importance chart")

print("\n🚀 Next Steps:")
print("   1. Tạo Flask API (xem file flask_api.py)")
print("   2. Chạy Flask server")
print("   3. Update AIService.cs trong HealthManagement để gọi API")
print("   4. Test prediction từ web application")

print("\n"+"="*70)
