# 📊 DANH SÁCH DATASETS CHO HEALTH MANAGEMENT AI

## 🎯 MỤC ĐÍCH
Tài liệu này liệt kê TẤT CẢ datasets phù hợp để train AI model cho các tính năng dự đoán trong ứng dụng HealthManagement.

---

## 🔴 DATASET CHÍNH - DỰ ĐOÁN BỆNH TIM (KHUYẾN NGHỊ)

### 1. **Heart Disease UCI** ⭐⭐⭐⭐⭐
**✅ KHỚP HOÀN HẢO 100% với form dự đoán hiện tại!**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset |
| **Alternative 1** | https://www.kaggle.com/datasets/redwankarimsony/heart-disease-data |
| **Alternative 2** | https://www.kaggle.com/datasets/fedesoriano/heart-failure-prediction |
| **Features** | Age, Sex, ChestPainType, RestingBP, Cholesterol, FastingBS, MaxHR, ExerciseAngina |
| **Số features** | 8 features (khớp với PredictionRequest) |
| **Target** | HeartDisease (0 = Không bệnh, 1 = Có bệnh) |
| **Samples** | ~900-1200 records |
| **Accuracy kỳ vọng** | **85-92%** 🎯 |
| **File size** | ~50KB |
| **Format** | CSV |
| **Missing values** | Có (cần xử lý) |
| **Class balance** | Tương đối cân bằng (45/55) |
| **Nguồn gốc** | UCI Machine Learning Repository |
| **Published** | 2020 |
| **Usability** | 10/10 ⭐ |

**✅ CHỌN DATASET NÀY - ĐÃ CÓ SCRIPT TRAIN SẴN!**

**Features mapping với ứng dụng:**
```
Dataset Feature    →  PredictionRequest (C#)
================      =====================
Age                →  Age
Sex                →  Sex (M/F hoặc 1/0)
ChestPainType      →  ChestPainType (0-3)
RestingBP          →  RestingBP (mmHg)
Cholesterol        →  Cholesterol (mg/dL)
FastingBS          →  FastingBS (0/1)
MaxHR              →  MaxHR (bpm)
ExerciseAngina     →  ExerciseAngina (0/1)
```

---

### 2. **Heart Disease Cleveland** ⭐⭐⭐⭐
**Dataset gốc từ Cleveland Clinic - Chất lượng cao**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/cherngs/heart-disease-cleveland-uci |
| **Features** | 13 features (clinical + lab tests) |
| **Target** | 5 classes (0-4, có thể binary hóa) |
| **Samples** | ~300 records |
| **Accuracy kỳ vọng** | 80-85% |
| **Đặc điểm** | Dataset nhỏ nhưng chất lượng cao, chuẩn medical |

---

### 3. **Cardiovascular Disease Dataset** ⭐⭐⭐⭐
**Dataset LỚN NHẤT - 70,000 records**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/sulianova/cardiovascular-disease-dataset |
| **Features** | Age, Gender, Height, Weight, AP (systolic/diastolic), Cholesterol, Glucose, Smoking, Alcohol, Activity |
| **Target** | Cardio (0/1) |
| **Samples** | **70,000 records** (RẤT LỚN) |
| **Accuracy kỳ vọng** | 72-78% |
| **Ưu điểm** | Dataset lớn → Model tổng quát hóa tốt |
| **Nhược điểm** | Features khác so với form hiện tại, cần mapping |

---

## 🟡 DATASETS BỔ SUNG - MỞ RỘNG CHỨC NĂNG

### 4. **Diabetes - PIMA Indians** ⭐⭐⭐⭐
**Mở rộng: Dự đoán tiểu đường**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/uciml/pima-indians-diabetes-database |
| **Alternative** | https://www.kaggle.com/datasets/mathchi/diabetes-data-set |
| **Features** | Pregnancies, Glucose, BloodPressure, SkinThickness, Insulin, BMI, DiabetesPedigreeFunction, Age |
| **Target** | Outcome (0 = No diabetes, 1 = Diabetes) |
| **Samples** | 768 records |
| **Accuracy kỳ vọng** | 75-82% |
| **Use case** | Thêm tính năng dự đoán tiểu đường vào app |

**Có thể sử dụng nếu:**
- Muốn thêm loại dự đoán mới: "Diabetes Risk"
- Có thể dùng BMI từ HoSoSucKhoe
- Glucose từ ChiSoSucKhoe

---

### 5. **Stroke Prediction** ⭐⭐⭐
**Mở rộng: Dự đoán đột quỵ**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/fedesoriano/stroke-prediction-dataset |
| **Features** | Age, Gender, Hypertension, Heart Disease, Ever Married, Work Type, Residence, Avg Glucose, BMI, Smoking |
| **Target** | Stroke (0/1) |
| **Samples** | 5,110 records |
| **Accuracy kỳ vọng** | 75-85% |
| **Class imbalance** | ⚠️ Rất imbalanced (5% stroke) - cần SMOTE |

---

### 6. **Chronic Kidney Disease** ⭐⭐⭐
**Mở rộng: Dự đoán bệnh thận**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/mansoordaku/ckdisease |
| **Features** | Blood Pressure, Specific Gravity, Albumin, Sugar, Red Blood Cells, etc. |
| **Target** | CKD (0/1) |
| **Samples** | 400 records |
| **Accuracy kỳ vọng** | 95-98% (dễ phân loại) |

---

### 7. **Liver Disease** ⭐⭐⭐
**Mở rộng: Dự đoán bệnh gan**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/uciml/indian-liver-patient-records |
| **Features** | Age, Gender, Total Bilirubin, Direct Bilirubin, Alkaline Phosphotase, Alamine Aminotransferase, etc. |
| **Target** | Liver Patient (1/2) |
| **Samples** | 583 records |
| **Accuracy kỳ vọng** | 70-75% |

---

## 🟢 DATASETS CHUYÊN SÂU (ADVANCED)

### 8. **Framingham Heart Study** ⭐⭐⭐⭐⭐
**Dataset y khoa chuẩn - 10-year CHD risk**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/aasheesh200/framingham-heart-study-dataset |
| **Features** | 15 clinical features |
| **Target** | 10-year CHD risk |
| **Samples** | ~4,200 records |
| **Đặc điểm** | Nghiên cứu y khoa lâu dài, chất lượng cao |

---

### 9. **Heart Attack Analysis & Prediction** ⭐⭐⭐
**Dataset tổng hợp nhiều nguồn**

| Thông tin | Chi tiết |
|-----------|----------|
| **Kaggle URL** | https://www.kaggle.com/datasets/rashikrahmanpritom/heart-attack-analysis-prediction-dataset |
| **Samples** | 303 records |
| **Features** | 14 features |
| **Target** | Output (0/1) |

---

## 📋 SO SÁNH VÀ LỰA CHỌN

| Dataset | Samples | Accuracy | Khớp App | Khuyến nghị |
|---------|---------|----------|----------|-------------|
| **Heart Disease UCI** | 1,200 | 85-92% | ✅✅✅ 100% | **⭐⭐⭐⭐⭐ CHỌN CÁI NÀY** |
| Heart Disease Cleveland | 300 | 80-85% | ✅✅ 90% | ⭐⭐⭐⭐ Tốt |
| Cardiovascular (70K) | 70,000 | 72-78% | ✅ 70% | ⭐⭐⭐ Dataset lớn |
| Diabetes PIMA | 768 | 75-82% | ⚠️ Khác | ⭐⭐⭐ Mở rộng |
| Stroke | 5,110 | 75-85% | ⚠️ Khác | ⭐⭐⭐ Mở rộng |
| Framingham | 4,200 | 80-88% | ✅✅ 85% | ⭐⭐⭐⭐ Advanced |

---

## 🎯 KHUYẾN NGHỊ CUỐI CÙNG

### **Giai đoạn 1: BẮT ĐẦU (NGAY BÂY GIỜ)** ✅
**Dataset:** Heart Disease UCI  
**Lý do:**
- ✅ Khớp 100% với form hiện tại
- ✅ Không cần thay đổi code C#
- ✅ Accuracy cao (85-92%)
- ✅ Đã có script train sẵn
- ✅ Dễ implement (1-2 giờ)

**Download:** https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset

---

### **Giai đoạn 2: MỞ RỘNG (SAU NÀY)**
Nếu muốn thêm tính năng:

1. **Diabetes Prediction:**
   - Dataset: PIMA Indians Diabetes
   - Thêm form mới với features: Glucose, BMI, Age
   - Tạo model thứ 2

2. **Stroke Prediction:**
   - Dataset: Stroke Prediction
   - Form khác với features khác
   - Model thứ 3

3. **Multi-Disease Model:**
   - Combine nhiều datasets
   - Ensemble models
   - Advanced AI features

---

### **Giai đoạn 3: NÂNG CAO (TỐI ƯU HÓA)**
- Deep Learning (Neural Networks)
- Time-series prediction (dự đoán xu hướng)
- Recommendation system (khuyến nghị lối sống)
- Computer Vision (phân tích hình ảnh y tế)

---

## 📥 HƯỚNG DẪN DOWNLOAD

### Cách 1: Web Browser (Đơn giản nhất)
1. Vào link Kaggle
2. Đăng nhập (tạo account free)
3. Click "Download" 
4. Giải nén vào `AIModel/data/`

### Cách 2: Kaggle API
```powershell
# Setup
pip install kaggle
# Vào https://www.kaggle.com/settings/account
# Click "Create New API Token" → kaggle.json
# Copy vào: C:\Users\<YourName>\.kaggle\

# Download
kaggle datasets download -d johnsmith88/heart-disease-dataset
unzip heart-disease-dataset.zip -d AIModel/data/
```

---

## ✅ CHECKLIST

- [ ] Đã đọc và hiểu tài liệu này
- [ ] Đã chọn dataset: **Heart Disease UCI**
- [ ] Đã tạo account Kaggle
- [ ] Đã download dataset
- [ ] File `heart.csv` đã có trong `AIModel/data/`
- [ ] Sẵn sàng chạy `train_heart_disease.py`

---

## 🔗 LINKS QUAN TRỌNG

1. **Dataset chính:** https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset
2. **UCI ML Repository:** https://archive.ics.uci.edu/ml/
3. **Kaggle Learn ML:** https://www.kaggle.com/learn/intro-to-machine-learning
4. **Scikit-learn Docs:** https://scikit-learn.org/

---

## 📞 HỖ TRỢ

**Vấn đề download dataset?**
- Thử alternative links
- Tìm kiếm trên Kaggle: "heart disease uci"
- Nhiều người upload dataset tương tự

**Dataset khác với doc?**
- Xem column names: `df.columns`
- Adjust script train: mapping features
- Hoặc dùng dataset khác trong danh sách

---

**🎉 CHÚC BẠN TRAIN MODEL THÀNH CÔNG!**
