"""
TEST TRAINED MODEL
Script để test model đã train với nhiều test cases
"""

import joblib
import pandas as pd
import numpy as np

print("="*70)
print("🧪 TESTING TRAINED MODEL")
print("="*70)

# Load models
print("\n📂 Loading models...")
try:
    model = joblib.load('heart_disease_model.pkl')
    scaler = joblib.load('heart_disease_scaler.pkl')
    feature_names = joblib.load('heart_disease_features.pkl')
    print("✅ Models loaded successfully!")
except Exception as e:
    print(f"❌ Error: {e}")
    print("Hãy chạy train_heart_disease.py trước!")
    exit()

# Test cases (khớp với form trong HealthManagement)
test_cases = [
    {
        'name': 'Test 1: Nguy cơ thấp - Người trẻ khỏe mạnh',
        'input': {
            'Age': 30,
            'Sex': 1,
            'ChestPainType': 0,
            'RestingBP': 110,
            'Cholesterol': 180,
            'FastingBS': 0,
            'MaxHR': 170,
            'ExerciseAngina': 0
        }
    },
    {
        'name': 'Test 2: Nguy cơ trung bình - Một số chỉ số cao',
        'input': {
            'Age': 50,
            'Sex': 1,
            'ChestPainType': 1,
            'RestingBP': 135,
            'Cholesterol': 220,
            'FastingBS': 0,
            'MaxHR': 140,
            'ExerciseAngina': 0
        }
    },
    {
        'name': 'Test 3: Nguy cơ cao - Nhiều chỉ số vượt ngưỡng',
        'input': {
            'Age': 65,
            'Sex': 1,
            'ChestPainType': 3,
            'RestingBP': 160,
            'Cholesterol': 280,
            'FastingBS': 1,
            'MaxHR': 100,
            'ExerciseAngina': 1
        }
    },
    {
        'name': 'Test 4: Nữ giới - Chỉ số trung bình',
        'input': {
            'Age': 45,
            'Sex': 0,
            'ChestPainType': 1,
            'RestingBP': 125,
            'Cholesterol': 200,
            'FastingBS': 0,
            'MaxHR': 155,
            'ExerciseAngina': 0
        }
    },
    {
        'name': 'Test 5: Người cao tuổi - Không triệu chứng',
        'input': {
            'Age': 70,
            'Sex': 0,
            'ChestPainType': 0,
            'RestingBP': 130,
            'Cholesterol': 190,
            'FastingBS': 0,
            'MaxHR': 120,
            'ExerciseAngina': 0
        }
    }
]

# Run tests
print("\n" + "="*70)
print("🔬 RUNNING TEST CASES")
print("="*70)

for i, test in enumerate(test_cases, 1):
    print(f"\n{'='*70}")
    print(f"📋 {test['name']}")
    print(f"{'='*70}")
    
    # Display input
    print("\n📝 Input:")
    for key, value in test['input'].items():
        print(f"   {key:20s}: {value}")
    
    # Prepare data
    input_df = pd.DataFrame([test['input']])
    
    # Ensure all features exist
    for feature in feature_names:
        if feature not in input_df.columns:
            input_df[feature] = 0
    
    # Order columns
    input_df = input_df[feature_names]
    
    # Scale and predict
    input_scaled = scaler.transform(input_df)
    prediction = model.predict(input_scaled)[0]
    
    print(f"\n🎯 Prediction:")
    print(f"   Class: {prediction} ({'❤️  DISEASE' if prediction == 1 else '✅ NO DISEASE'})")
    
    # Get probability if available
    if hasattr(model, 'predict_proba'):
        probabilities = model.predict_proba(input_scaled)[0]
        prob_no_disease = probabilities[0] * 100
        prob_disease = probabilities[1] * 100
        
        print(f"   Probability:")
        print(f"      No Disease: {prob_no_disease:.2f}%")
        print(f"      Disease:    {prob_disease:.2f}%")
        
        # Risk level
        if prob_disease >= 70:
            risk = "🔴 HIGH RISK"
        elif prob_disease >= 40:
            risk = "🟡 MEDIUM RISK"
        else:
            risk = "🟢 LOW RISK"
        
        print(f"\n   Risk Level: {risk}")

print("\n" + "="*70)
print("✅ ALL TESTS COMPLETED")
print("="*70)

# Summary
print("\n📊 Model Information:")
print(f"   Model Type: {type(model).__name__}")
print(f"   Features: {len(feature_names)}")
print(f"   Feature List: {feature_names}")

print("\n🎓 Interpretation Guide:")
print("   Risk < 40%:  Nguy cơ thấp - Tiếp tục duy trì lối sống lành mạnh")
print("   Risk 40-70%: Nguy cơ trung bình - Cần cải thiện một số thói quen")
print("   Risk > 70%:  Nguy cơ cao - Khuyến nghị gặp bác sĩ")
