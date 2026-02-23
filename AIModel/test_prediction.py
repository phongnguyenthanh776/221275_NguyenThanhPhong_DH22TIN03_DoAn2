import requests
import json

# Test Stroke prediction
print("\n🧠 Testing STROKE prediction...")
stroke_data = {
    'disease_type': 'stroke',
    'age': 67,
    'gender': 1,
    'hypertension': 0,
    'heart_disease': 1,
    'avg_blood_pressure': 156.0,
    'bmi': 25.3,
    'smoking': 1
}

resp = requests.post('http://localhost:5000/predict', json=stroke_data)
result = resp.json()
print(f"  Disease: {result.get('DiseaseType')}")
print(f"  Risk Level: {result.get('RiskLevel')}%")
print(f"  Status: {result.get('Result')}")

# Test Diabetes prediction
print("\n💙 Testing DIABETES prediction...")
diabetes_data = {
    'disease_type': 'diabetes',
    'pregnancies': 1,
    'glucose': 120,
    'blood_pressure': 80,
    'skin_thickness': 20,
    'insulin': 100,
    'bmi': 28.0,
    'diabetes_pedigree_function': 0.5,
    'age': 45
}

resp = requests.post('http://localhost:5000/predict', json=diabetes_data)
result = resp.json()
print(f"  Disease: {result.get('DiseaseType')}")
print(f"  Risk Level: {result.get('RiskLevel')}%")
print(f"  Status: {result.get('Result')}")

# Test Heart Disease prediction
print("\n❤️ Testing HEART DISEASE prediction...")
heart_data = {
    'disease_type': 'heart_disease',
    'age': 55,
    'sex': 1,
    'cp': 2,
    'trestbps': 140,
    'chol': 250,
    'fbs': 1,
    'thalach': 150,
    'exang': 0,
    'oldpeak': 1.5,
    'slope': 1,
    'ca': 1,
    'thal': 3
}

resp = requests.post('http://localhost:5000/predict', json=heart_data)
result = resp.json()
print(f"  Disease: {result.get('DiseaseType')}")
print(f"  Risk Level: {result.get('RiskLevel')}%")
print(f"  Status: {result.get('Result')}")

print("\n✅ All predictions completed!\n")
