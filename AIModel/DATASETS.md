# DATASETS CHO AI MODEL

## Tổng quan

Hệ thống sử dụng **6 bộ dữ liệu** cho 6 bệnh được dự đoán, chia thành hai nhóm:

**Nhóm tabular (file CSV)** — nằm trong `AIModel/data/`:
- `heart.csv` — Bệnh tim mạch
- `diabetes.csv` — Tiểu đường
- `cardio_train.csv` — Huyết áp cao / tim mạch
- `stroke.csv` — Đột quỵ

**Nhóm ảnh** — nằm trong `AIModel/data/`:
- `chest_xray/` — Viêm phổi (NORMAL / PNEUMONIA)
- `CT-KIDNEY-DATASET-Normal-Cyst-Tumor-Stone/` — Sỏi thận (Normal / Stone)

## Chi tiết từng dataset

### Bệnh tim
- Nguồn: Kaggle — Heart Disease Dataset
- File: `heart.csv`
- Đặc trưng chính: age, sex, cp, trestbps, chol, fbs, thalach, exang

### Tiểu đường
- Nguồn: Kaggle — Pima Indians Diabetes Database
- File: `diabetes.csv`
- Đặc trưng chính: Pregnancies, Glucose, BloodPressure, SkinThickness, Insulin, BMI, DiabetesPedigreeFunction, Age

### Huyết áp cao
- Nguồn: Kaggle — Cardiovascular Disease Dataset
- File: `cardio_train.csv`
- Đặc trưng chính: age, gender, ap_hi, ap_lo, cholesterol, gluc, smoke, alco, active

### Đột quỵ
- Nguồn: Kaggle — Stroke Prediction Dataset
- File: `stroke.csv`
- Đặc trưng chính: age, gender, hypertension, heart_disease, avg_glucose_level, bmi, smoking_status
- Lưu ý: Tỷ lệ mất cân bằng nghiêm trọng (~5% ca đột quỵ), script train dùng chiến lược đặc biệt với cross-validation và threshold hạ thấp.

### Sỏi thận (ảnh CNN)
- Nguồn: Kaggle — CT KIDNEY DATASET: Normal-Cyst-Tumor-Stone
- Thư mục: `CT-KIDNEY-DATASET-Normal-Cyst-Tumor-Stone/CT-KIDNEY-DATASET-Normal-Cyst-Tumor-Stone/`
- Lớp được dùng: `Normal` (không sỏi) và `Stone` (có sỏi) — tổng **6.454 ảnh**
- Lớp Cyst và Tumor không được đưa vào vì thuộc bài toán khác nhãn.

### Viêm phổi (ảnh CNN)
- Nguồn: Kaggle — Chest X-Ray Images (Pneumonia)
- Thư mục: `chest_xray/` — có sẵn split `train/`, `val/`, `test/`
- Lớp: `NORMAL` và `PNEUMONIA`
- Script train gom **toàn bộ ảnh** từ tất cả split (train 5.216 + val 16 + test 624 = **5.856 ảnh**) rồi tách lại nội bộ theo tỷ lệ 85/15.

## Lưu ý

Script train tự động mapping và chuẩn hóa tên cột trong quá trình huấn luyện. Flask API đọc các file model theo mẫu đặt tên: `heart_disease_model.pkl`, `diabetes_model.pkl`, `hypertension_model.pkl`, `stroke_model.pkl`, `kidney_stone_image_model.pt`, `pneumonia_image_model.pt`.
