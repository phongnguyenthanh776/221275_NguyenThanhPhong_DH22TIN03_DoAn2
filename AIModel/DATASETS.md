# DATASETS CHO AI MODEL

## Tong quan

Hien tai he thong su dung 4 bo du lieu tuong ung 4 benh:
- heart.csv (Benh tim)
- diabetes.csv (Tieu duong)
- cardio_train.csv (Huyet ap)
- stroke.csv (Dot quy)

Cac file nay dang nam trong thu muc AIModel/data.

## De xuat dataset

### Benh tim
- Kaggle: https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset
- File de xuat: heart.csv
- Features hay gap: age, sex, cp, trestbps, chol, fbs, thalach, exang

### Tieu duong
- Kaggle: https://www.kaggle.com/datasets/uciml/pima-indians-diabetes-database
- File de xuat: diabetes.csv
- Features: Pregnancies, Glucose, BloodPressure, SkinThickness, Insulin, BMI, DiabetesPedigreeFunction, Age

### Huyet ap
- Kaggle: https://www.kaggle.com/datasets/sulianova/cardiovascular-disease-dataset
- File de xuat: cardio_train.csv
- Features chinh: age, gender, ap_hi, ap_lo, cholesterol, gluc, smoke, alco, active

### Dot quy
- Kaggle: https://www.kaggle.com/datasets/fedesoriano/stroke-prediction-dataset
- File de xuat: stroke.csv
- Features chinh: age, gender, hypertension, heart_disease, avg_glucose_level, bmi, smoking_status

## Luu y

- Script train se tu dong mapping/normalize ten cot trong qua trinh train.
- Flask API se doc cac file model theo mau:
  - heart_disease_model.pkl
  - diabetes_model.pkl
  - hypertension_model.pkl
  - stroke_model.pkl
