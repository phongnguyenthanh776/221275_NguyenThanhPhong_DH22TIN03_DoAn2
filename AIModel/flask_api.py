"""
FLASK API SERVER FOR HEART DISEASE PREDICTION
Endpoint tích hợp với HealthManagement C# Application
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
import joblib
import numpy as np
import pandas as pd
import os

app = Flask(__name__)
CORS(app)  # Enable CORS for C# application

# ============================================================
# LOAD TRAINED MODELS
# ============================================================
print("🚀 Loading trained models...")

try:
    model = joblib.load('heart_disease_model.pkl')
    scaler = joblib.load('heart_disease_scaler.pkl')
    feature_names = joblib.load('heart_disease_features.pkl')
    print("✅ Models loaded successfully!")
    print(f"✅ Model type: {type(model).__name__}")
    print(f"✅ Features: {feature_names}")
except Exception as e:
    print(f"❌ Error loading models: {e}")
    print("📝 Hãy chạy train_heart_disease.py trước!")
    exit()

# ============================================================
# MAPPING FEATURES
# ============================================================
# Mapping từ PredictionRequest (C# - JSON camelCase) sang features của model
# C# JSON serialization tự động chuyển sang camelCase: Age -> age, ChestPainType -> chestPainType
FEATURE_MAPPING = {
    'age': 'age',                    # Age
    'sex': 'sex',                    # Sex (will convert Male/Female -> 1/0)
    'chestPainType': 'cp',           # ChestPainType -> cp
    'restingBP': 'trestbps',         # RestingBP -> trestbps (resting blood pressure)
    'cholesterol': 'chol',           # Cholesterol -> chol
    'fastingBS': 'fbs',              # FastingBS -> fbs (fasting blood sugar)
    'maxHR': 'thalach',              # MaxHR -> thalach (maximum heart rate)
    'exerciseAngina': 'exang'        # ExerciseAngina -> exang
}

# Default values for missing features (model has 13 features, form has 8)
DEFAULT_FEATURE_VALUES = {
    'restecg': 0,      # Resting ECG results (not in form)
    'oldpeak': 0,      # ST depression (not in form)
    'slope': 1,        # Slope of peak exercise ST segment (not in form)
    'ca': 0,           # Number of major vessels (not in form)
    'thal': 2          # Thalassemia (not in form, 2 = normal)
}

# ============================================================
# HELPER FUNCTIONS
# ============================================================
def convert_sex_to_numeric(sex_value):
    """Convert sex from string to numeric"""
    if isinstance(sex_value, str):
        sex_value = sex_value.strip().lower()
        if sex_value in ['male', 'm', 'nam']:
            return 1
        elif sex_value in ['female', 'f', 'nữ', 'nu']:
            return 0
    return int(sex_value)

def get_risk_level_and_recommendation(probability):
    """Xác định mức độ nguy cơ và lời khuyên"""
    risk_percentage = probability * 100
    
    if probability >= 0.7:
        result = "Nguy cơ cao"
        recommendation = "Khuyến nghị gặp bác sĩ khám chuyên khoa ngay. Cần kiểm tra sức khỏe toàn diện."
        details = "Model AI dự đoán nguy cơ bệnh tim cao. Cần theo dõi sát sao và điều chỉnh lối sống."
    elif probability >= 0.4:
        result = "Nguy cơ trung bình"
        recommendation = "Nên cải thiện lối sống: ăn uống lành mạnh, tập thể dục đều đặn, giảm stress."
        details = "Model AI phát hiện một số yếu tố nguy cơ. Theo dõi định kỳ và duy trì thói quen tốt."
    else:
        result = "Nguy cơ thấp"
        recommendation = "Tiếp tục duy trì lối sống lành mạnh. Khám sức khỏe định kỳ 6 tháng/lần."
        details = "Model AI đánh giá các chỉ số trong giới hạn bình thường. Tốt!"
    
    return result, risk_percentage, recommendation, details

# ============================================================
# API ENDPOINTS
# ============================================================

@app.route('/')
def home():
    """Home endpoint"""
    return jsonify({
        'message': 'Heart Disease Prediction API',
        'status': 'running',
        'model': type(model).__name__,
        'version': '1.0',
        'endpoints': {
            '/predict': 'POST - Predict heart disease risk',
            '/health': 'GET - Check API health'
        }
    })

@app.route('/health')
def health():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'model_loaded': model is not None,
        'scaler_loaded': scaler is not None
    })

@app.route('/predict', methods=['POST'])
def predict():
    """
    Predict heart disease risk
    
    Expected JSON body (khớp với PredictionRequest trong C#):
    {
        "Age": 55,
        "Sex": "Male" or 1,
        "ChestPainType": 2,
        "RestingBP": 140,
        "Cholesterol": 250,
        "FastingBS": 1,
        "MaxHR": 150,
        "ExerciseAngina": 0
    }
    
    Returns (khớp với PredictionResponse trong C#):
    {
        "Result": "Nguy cơ cao/trung bình/thấp",
        "RiskLevel": 75.5,
        "Recommendation": "...",
        "Details": "..."
    }
    """
    try:
        # Parse request
        data = request.get_json()
        
        if not data:
            return jsonify({
                'error': 'No data provided',
                'message': 'Request body must be JSON'
            }), 400
        
        print(f"\n📥 Received request: {data}")
        
        # Convert sex to numeric if needed
        if 'sex' in data:
            data['sex'] = convert_sex_to_numeric(data['sex'])
        
        # Create input data with mapping from API keys to model features
        input_data = {}
        
        # Map received data to model features
        for api_key, model_key in FEATURE_MAPPING.items():
            if api_key in data:
                input_data[model_key] = data[api_key]
        
        # Add default values for missing features
        for feature, default_value in DEFAULT_FEATURE_VALUES.items():
            if feature not in input_data:
                input_data[feature] = default_value
        
        # Convert to DataFrame
        input_df = pd.DataFrame([input_data])
        
        # Ensure all model features exist
        for feature in feature_names:
            if feature not in input_df.columns:
                input_df[feature] = 0
        
        # Order columns according to training data
        input_df = input_df[feature_names]
        
        print(f"📊 Input features (mapped): {input_df.to_dict('records')[0]}")
        
        # Scale features
        input_scaled = scaler.transform(input_df)
        
        # Predict
        prediction = model.predict(input_scaled)[0]
        probability = None
        
        if hasattr(model, 'predict_proba'):
            probabilities = model.predict_proba(input_scaled)[0]
            probability = probabilities[1]  # Probability of class 1 (disease)
            print(f"🎯 Prediction: {prediction}, Probability: {probability:.4f}")
        else:
            print(f"🎯 Prediction: {prediction}")
            # Nếu model không có predict_proba, dùng prediction binary
            probability = 0.8 if prediction == 1 else 0.2
        
        # Get risk level and recommendation
        result, risk_level, recommendation, details = get_risk_level_and_recommendation(probability)
        
        # Create response (khớp với PredictionResponse trong C#)
        response = {
            'Result': result,
            'RiskLevel': float(risk_level),
            'Recommendation': recommendation,
            'Details': details
        }
        
        print(f"📤 Response: {response}")
        
        return jsonify(response), 200
        
    except Exception as e:
        print(f"❌ Error: {str(e)}")
        import traceback
        traceback.print_exc()
        
        return jsonify({
            'error': str(e),
            'message': 'Prediction failed'
        }), 500

# ============================================================
# RUN SERVER
# ============================================================
if __name__ == '__main__':
    print("\n" + "="*70)
    print("🚀 STARTING FLASK API SERVER")
    print("="*70)
    print(f"✅ Model: {type(model).__name__}")
    print(f"✅ Features: {len(feature_names)}")
    print(f"✅ Server: http://localhost:5000")
    print("\n📍 Endpoints:")
    print("   GET  /         - API info")
    print("   GET  /health   - Health check")
    print("   POST /predict  - Heart disease prediction")
    print("\n🔗 Tích hợp với HealthManagement:")
    print("   Cập nhật appsettings.json:")
    print('   "AISettings": {')
    print('     "IsEnabled": true,')
    print('     "ApiUrl": "http://localhost:5000/predict"')
    print('   }')
    print("\n" + "="*70 + "\n")
    
    # Run server
    app.run(host='0.0.0.0', port=5000, debug=True)
