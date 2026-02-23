"""
FLASK API SERVER FOR MULTIPLE DISEASE PREDICTIONS
Hỗ trợ dự đoán 4 loại bệnh: Tim, Tiểu đường, Huyết áp cao, Đột quỵ
Endpoint tích hợp với HealthManagement C# Application
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
import joblib
import numpy as np
import pandas as pd
import os
import builtins

def safe_print(*args, **kwargs):
    try:
        builtins.print(*args, **kwargs)
    except UnicodeEncodeError:
        normalized_args = [str(arg).encode('ascii', errors='ignore').decode('ascii') for arg in args]
        builtins.print(*normalized_args, **kwargs)

print = safe_print

app = Flask(__name__)
CORS(app)  # Enable CORS for C# application

# ============================================================
# LOAD TRAINED MODELS
# ============================================================
print("Loading trained models...")

MODELS = {}
SCALERS = {}
FEATURE_SETS = {}
DISEASE_TYPES = ['heart_disease', 'diabetes', 'hypertension', 'stroke']

for disease in DISEASE_TYPES:
    try:
        model_dir = os.path.dirname(os.path.abspath(__file__))
        model_file = os.path.join(model_dir, f'{disease}_model.pkl')
        scaler_file = os.path.join(model_dir, f'{disease}_scaler.pkl')
        features_file = os.path.join(model_dir, f'{disease}_features.pkl')

        if os.path.exists(model_file) and os.path.exists(scaler_file) and os.path.exists(features_file):
            MODELS[disease] = joblib.load(model_file)
            SCALERS[disease] = joblib.load(scaler_file)
            FEATURE_SETS[disease] = joblib.load(features_file)
            print(f"✅ {disease.upper()} model loaded!")
        else:
            print(f"⚠️  {disease.upper()} model files not found - training needed")
    except Exception as e:
        print(f"⚠️  Error loading {disease}: {e}")

if not MODELS:
    print("❌ No models loaded! Please run training scripts first.")
    print("   python train_heart_disease.py")
    print("   python train_diabetes.py")
    print("   python train_hypertension.py")
    print("   python train_stroke.py")

# ============================================================
# DISEASE DESCRIPTIONS
# ============================================================
DISEASE_INFO = {
    'heart_disease': {
        'name': 'Bệnh Tim (Heart Disease)',
        'description': 'Dự đoán nguy cơ bệnh tim dựa trên các chỉ số lâm sàng',
        'features': ['age', 'sex', 'chestpaintype', 'restingbp', 'cholesterol', 
                    'fastingbs', 'maxhr', 'exerciseangina']
    },
    'diabetes': {
        'name': 'Bệnh Tiểu Đường (Diabetes)',
        'description': 'Dự đoán nguy cơ bệnh tiểu đường',
        'features': ['pregnancies', 'glucose', 'bloodpressure', 'skinthickness',
                    'insulin', 'bmi', 'diabetespedigreefunction', 'age']
    },
    'hypertension': {
        'name': 'Huyết Áp Cao (Hypertension)',
        'description': 'Dự đoán nguy cơ tăng huyết áp',
        'features': ['age', 'gender', 'bmi', 'cholesterol', 'systolicbp',
                    'diastolicbp', 'heartrate', 'smoking', 'alcohol', 'physicalactivity']
    },
    'stroke': {
        'name': 'Đột Quỵ (Stroke)',
        'description': 'Dự đoán nguy cơ đột quỵ',
        'features': ['age', 'gender', 'hypertension', 'heartdisease', 'smoking',
                    'bmi', 'avgbloodpressure', 'glucose']
    }
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
    return int(sex_value) if sex_value else 0

def normalize_key(key):
    return ''.join(ch for ch in str(key).strip().lower() if ch.isalnum())

def to_numeric_value(value):
    if value is None:
        return 0.0
    if isinstance(value, (int, float, np.integer, np.floating)):
        if pd.isna(value):
            return 0.0
        return float(value)

    text = str(value).strip().lower()
    if text in ['male', 'm', 'nam']:
        return 1.0
    if text in ['female', 'f', 'nữ', 'nu']:
        return 0.0
    if text in ['yes', 'true', 'co']:
        return 1.0
    if text in ['no', 'false', 'khong', 'không']:
        return 0.0

    try:
        return float(text)
    except Exception:
        return 0.0

FEATURE_ALIASES = {
    'heart_disease': {
        'chestpaintype': 'cp',
        'restingbp': 'trestbps',
        'cholesterol': 'chol',
        'fastingbs': 'fbs',
        'maxhr': 'thalach',
        'exerciseangina': 'exang'
    },
    'stroke': {
        'avgbloodpressure': 'AvgBloodPressure'
    }
}

def build_input_dataframe(disease_type, disease_data, feature_names, scaler):
    if not isinstance(disease_data, dict):
        disease_data = {}

    normalized_input = {
        normalize_key(k): v for k, v in disease_data.items()
    }

    alias_map = FEATURE_ALIASES.get(disease_type, {})
    alias_pairs = [
        (normalize_key(src), normalize_key(dst)) for src, dst in alias_map.items()
    ]

    scaler_means = getattr(scaler, 'mean_', None)
    row = {}

    for index, feature_name in enumerate(feature_names):
        feature_norm = normalize_key(feature_name)
        value = None

        if feature_norm in normalized_input:
            value = normalized_input[feature_norm]
        else:
            for src_norm, dst_norm in alias_pairs:
                if dst_norm == feature_norm and src_norm in normalized_input:
                    value = normalized_input[src_norm]
                    break

        if value is None:
            if scaler_means is not None and len(scaler_means) > index:
                value = scaler_means[index]
            else:
                value = 0

        row[feature_name] = to_numeric_value(value)

    return pd.DataFrame([row], columns=feature_names)

def get_risk_level(probability, disease_type):
    """Xác định mức độ nguy cơ và lời khuyên dựa trên loại bệnh"""
    risk_percentage = probability * 100
    disease_name = DISEASE_INFO.get(disease_type, {}).get('name', 'bệnh')
    
    if probability >= 0.7:
        result = "Nguy cơ cao"
        recommendation = f"⚠️ Khuyến nghị gặp bác sĩ khám chuyên khoa ngay để kiểm tra {disease_name}."
        details = f"Model AI dự đoán nguy cơ {disease_name} cao ({risk_percentage:.1f}%). Cần theo dõi sát sao."
    elif probability >= 0.4:
        result = "Nguy cơ trung bình"
        recommendation = f"Nên cải thiện lối sống: ăn uống lành mạnh, tập thể dục đều đặn, giảm stress."
        details = f"Model AI phát hiện nguy cơ {disease_name} trung bình ({risk_percentage:.1f}%). Theo dõi định kỳ."
    else:
        result = "Nguy cơ thấp"
        recommendation = f"Tiếp tục duy trì lối sống lành mạnh. Khám sức khỏe định kỳ."
        details = f"Model AI đánh giá nguy cơ {disease_name} thấp ({risk_percentage:.1f}%). Tốt!"
    
    return result, risk_percentage, recommendation, details

# ============================================================
# API ENDPOINTS
# ============================================================

@app.route('/')
def home():
    """Home endpoint"""
    return jsonify({
        'message': 'Health Disease Prediction API (Multi-disease)',
        'status': 'running',
        'models_loaded': list(MODELS.keys()),
        'version': '2.0',
        'endpoints': {
            '/predict': 'POST - Predict disease risk',
            '/diseases': 'GET - List available diseases',
            '/health': 'GET - Check API health'
        }
    })

@app.route('/health')
def health():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'models_loaded': len(MODELS),
        'available_diseases': list(MODELS.keys())
    })

@app.route('/diseases', methods=['GET'])
def diseases():
    """List available diseases and their info"""
    return jsonify(DISEASE_INFO)

@app.route('/predict', methods=['POST'])
def predict():
    """
    Predict disease risk
    
    Expected JSON body:
    {
        "DiseaseType": "heart_disease|diabetes|hypertension|stroke",
        "Data": {
            // specific fields based on disease type
        }
    }
    
    Returns:
    {
        "Result": "Nguy cơ cao/trung bình/thấp",
        "RiskLevel": 75.5,
        "Recommendation": "...",
        "Details": "..."
    }
    """
    try:
        # Parse request
        request_data = request.get_json()
        
        if not request_data:
            return jsonify({
                'error': 'No data provided',
                'message': 'Request body must be JSON'
            }), 400
        
        # Get disease type (support both DiseaseType and disease_type)
        disease_type = request_data.get('DiseaseType') or request_data.get('disease_type', 'heart_disease')
        disease_type = disease_type.lower()
        disease_data = request_data.get('Data') or request_data.get('data') or request_data
        
        if disease_type not in MODELS:
            return jsonify({
                'error': f'Disease type "{disease_type}" not supported',
                'supported_types': list(MODELS.keys())
            }), 400
        
        print(f"\n📥 Prediction request for {disease_type}")
        print(f"   Data: {disease_data}")
        
        # Get model components
        model = MODELS[disease_type]
        scaler = SCALERS[disease_type]
        feature_names = FEATURE_SETS[disease_type]
        
        # Create input dataframe with robust feature mapping
        input_df = build_input_dataframe(disease_type, disease_data, feature_names, scaler)
        
        print(f"   Features: {list(input_df.columns)}")
        
        # Scale features
        input_scaled = scaler.transform(input_df)
        
        # Predict
        prediction = model.predict(input_scaled)[0]
        probability = None
        
        if hasattr(model, 'predict_proba'):
            probabilities = model.predict_proba(input_scaled)[0]
            probability = probabilities[1]  # Probability of class 1 (disease)
        else:
            probability = 0.8 if prediction == 1 else 0.2
        
        # Get risk level and recommendation
        result, risk_level, recommendation, details = get_risk_level(probability, disease_type)
        
        # Create response
        response = {
            'DiseaseType': disease_type,
            'Result': result,
            'RiskLevel': float(risk_level),
            'Recommendation': recommendation,
            'Details': details
        }
        
        print(f"   ✅ Response: {result} ({risk_level:.1f}%)")
        
        return jsonify(response), 200
        
    except Exception as e:
        print(f"❌ Prediction error: {str(e)}")
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
    print("🚀 STARTING MULTI-DISEASE PREDICTION API SERVER")
    print("="*70)
    print(f"\n✅ Available Models: {len(MODELS)}")
    for disease in MODELS.keys():
        disease_name = DISEASE_INFO.get(disease, {}).get('name', disease)
        print(f"   - {disease_name}")
    
    print(f"\n🔗 Server: http://localhost:5000")
    print("\n📍 Endpoints:")
    print("   GET  /              - API info")
    print("   GET  /health        - Health check")
    print("   GET  /diseases      - List available diseases")
    print("   POST /predict       - Disease prediction")
    
    print("\n💡 Example POST /predict:")
    print("""
    {
        "DiseaseType": "heart_disease",
        "Data": {
            "age": 55,
            "sex": "Male",
            "chestpaintype": 2,
            "restingbp": 140,
            "cholesterol": 250,
            "fastingbs": 1,
            "maxhr": 150,
            "exerciseangina": 0
        }
    }
    """)
    
    print("\n🔐 Configuration (appsettings.json):")
    print("""
    "AISettings": {
        "IsEnabled": true,
        "ApiUrl": "http://localhost:5000/predict"
    }
    """)
    
    print("\n" + "="*70 + "\n")
    
    # Run server
    app.run(host='0.0.0.0', port=5000, debug=True)
