# AI MODEL TRAINING & API

## Muc tieu

Huong dan train 4 model va chay Flask API cho he thong Health Management.

## Cau truc thu muc

AIModel/
- data/ (cac file csv)
- train_heart_disease.py
- train_diabetes.py
- train_hypertension.py
- train_stroke.py
- flask_api.py

## Cai dat moi truong

```
cd AIModel
python -m venv venv
.\venv\Scripts\activate
pip install -r requirements.txt
```

## Train model

Chay lan luot 4 script:

```
python train_heart_disease.py
python train_diabetes.py
python train_hypertension.py
python train_stroke.py
```

Sau khi train, se co cac file:
- heart_disease_model.pkl
- heart_disease_scaler.pkl
- heart_disease_features.pkl
- diabetes_model.pkl
- diabetes_scaler.pkl
- diabetes_features.pkl
- hypertension_model.pkl
- hypertension_scaler.pkl
- hypertension_features.pkl
- stroke_model.pkl
- stroke_scaler.pkl
- stroke_features.pkl

## Chay Flask API

```
python flask_api.py
```

Kiem tra nhanh:
- GET http://localhost:5000/health

## Goi API du doan

POST http://localhost:5000/predict

Body JSON:
```
{
  "disease_type": "heart_disease",
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
```

Disease types ho tro:
- heart_disease
- diabetes
- hypertension
- stroke

## Tich hop voi C#

Trong HealthManagement/appsettings.json:

```
"AISettings": {
  "ApiUrl": "http://localhost:5000/predict",
  "IsEnabled": true,
  "TimeoutSeconds": 30
}
```

## Luu y

- Flask API tu dong mapping ten cot va fill gia tri thieu theo scaler.
- Neu chua co model pkl, API se thong bao thieu model.
