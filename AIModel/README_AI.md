# 🤖 AI MODEL TRAINING & DEPLOYMENT

## 📋 MỤC LỤC
1. [Datasets Cần Thiết](#datasets-cần-thiết)
2. [Cài Đặt Môi Trường](#cài-đặt-môi-trường)
3. [Download Dataset](#download-dataset)
4. [Train Model](#train-model)
5. [Chạy Flask API](#chạy-flask-api)
6. [Tích Hợp Với C# Application](#tích-hợp-với-c-application)

---

## 📊 DATASETS CẦN THIẾT

### 🔴 **DATASET CHÍNH (BẮT BUỘC)**

#### **1. Heart Disease Classification** ⭐⭐⭐⭐⭐
**100% phù hợp** với form dự đoán trong HealthManagement!

**📥 Kaggle Links (Chọn 1):**
- **Khuyến nghị:** https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset
- **Alternative 1:** https://www.kaggle.com/datasets/redwankarimsony/heart-disease-data
- **Alternative 2:** https://www.kaggle.com/datasets/fedesoriano/heart-failure-prediction

**✅ Features khớp hoàn toàn:**
```
Age, Sex, ChestPainType, RestingBP, Cholesterol, 
FastingBS, MaxHR, ExerciseAngina
```

**🎯 Target:** HeartDisease (0 hoặc 1)
**📦 Size:** ~900-1200 records
**🎪 Expected Accuracy:** 85-92%

---

### 🟡 **DATASET BỔ SUNG (OPTIONAL)**

#### **2. Diabetes Prediction**
- **Link:** https://www.kaggle.com/datasets/uciml/pima-indians-diabetes-database
- **Use case:** Mở rộng dự đoán tiểu đường
- **Features:** Glucose, BloodPressure, BMI, Age
- **Size:** 768 records

#### **3. Cardiovascular Disease**
- **Link:** https://www.kaggle.com/datasets/sulianova/cardiovascular-disease-dataset
- **Use case:** Dataset lớn hơn (70K records)
- **Features:** Age, Gender, BP, Cholesterol, Smoking

#### **4. Stroke Prediction**
- **Link:** https://www.kaggle.com/datasets/fedesoriano/stroke-prediction-dataset
- **Use case:** Dự đoán đột quỵ
- **Size:** 5110 records

---

## 🛠️ CÀI ĐẶT MÔI TRƯỜNG

### Bước 1: Kiểm tra Python
```powershell
python --version
# Cần: Python 3.8 trở lên
```

### Bước 2: Tạo Virtual Environment (Khuyến nghị)
```powershell
cd AIModel
python -m venv venv
.\venv\Scripts\activate
```

### Bước 3: Cài Đặt Packages
```powershell
pip install -r requirements.txt
```

**Hoặc cài từng package:**
```powershell
pip install pandas numpy scikit-learn matplotlib seaborn joblib flask flask-cors kaggle
```

### Bước 4: Kiểm tra cài đặt
```powershell
python -c "import sklearn; print(sklearn.__version__)"
```

---

## 📥 DOWNLOAD DATASET

### **Cách 1: Download Trực Tiếp (ĐƠN GIẢN NHẤT)** ✅

1. **Vào Kaggle:**
   - Mở: https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset
   - Đăng nhập Kaggle (tạo tài khoản nếu chưa có)

2. **Download:**
   - Click nút **Download** ở góc phải
   - Sẽ tải file `heart-disease-dataset.zip`

3. **Giải nén:**
   ```powershell
   # Tạo folder data
   mkdir data
   
   # Giải nén file ZIP vào folder data
   # (Dùng WinRAR, 7-Zip hoặc click chuột phải → Extract)
   # Kết quả: AIModel/data/heart.csv
   ```

### **Cách 2: Dùng Kaggle API** (CHO PRO)

1. **Setup Kaggle Credentials:**
   - Vào: https://www.kaggle.com/settings/account
   - Scroll xuống phần **API**
   - Click **Create New API Token**
   - Sẽ download file `kaggle.json`

2. **Configure:**
   ```powershell
   # Windows: Copy vào
   mkdir $env:USERPROFILE\.kaggle
   copy kaggle.json $env:USERPROFILE\.kaggle\
   ```

3. **Download Dataset:**
   ```powershell
   # Install Kaggle CLI
   pip install kaggle
   
   # Download heart disease dataset
   kaggle datasets download -d johnsmith88/heart-disease-dataset
   
   # Giải nén
   unzip heart-disease-dataset.zip -d data/
   ```

### **Xác nhận dataset đã tải:**
```powershell
ls data/
# Phải thấy: heart.csv (hoặc tên tương tự)
```

---

## 🚀 TRAIN MODEL

### Bước 1: Chạy Training Script
```powershell
cd AIModel
python train_heart_disease.py
```

### Bước 2: Theo dõi quá trình
Script sẽ tự động:
- ✅ Load dataset
- ✅ Explore dữ liệu
- ✅ Preprocessing
- ✅ Train 4 models: Logistic Regression, Random Forest, Gradient Boosting, SVM
- ✅ So sánh accuracy
- ✅ Chọn model tốt nhất
- ✅ Lưu model vào file `.pkl`
- ✅ Tạo visualizations (charts)

### Bước 3: Kiểm tra output
Sau khi chạy xong, sẽ có các files:
```
AIModel/
├── heart_disease_model.pkl       # Model đã train
├── heart_disease_scaler.pkl      # Scaler để chuẩn hóa input
├── heart_disease_features.pkl    # Danh sách features
├── model_comparison.png          # So sánh các models
├── confusion_matrix.png          # Ma trận nhầm lẫn
└── feature_importance.png        # Độ quan trọng features
```

### Expected Results:
```
🏆 BEST MODEL RESULTS
====================================
✅ Best Model: Random Forest
✅ Accuracy: 0.8750 (87.50%)
```

**Giải thích:**
- **Accuracy 85-92%**: RẤT TỐT cho medical prediction
- **Model tốt nhất**: Thường là Random Forest hoặc Gradient Boosting
- **Cross-validation**: Đảm bảo model không overfitting

---

## 🌐 CHẠY FLASK API

### Bước 1: Start API Server
```powershell
cd AIModel
python flask_api.py
```

### Bước 2: Kiểm tra API hoạt động
Mở browser hoặc Postman test:
```
GET http://localhost:5000/
GET http://localhost:5000/health
```

### Bước 3: Test Prediction
**Dùng PowerShell:**
```powershell
$body = @{
    Age = 55
    Sex = 'Male'
    ChestPainType = 2
    RestingBP = 140
    Cholesterol = 250
    FastingBS = 1
    MaxHR = 150
    ExerciseAngina = 0
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/predict" -Method POST -Body $body -ContentType "application/json"
```

**Expected Response:**
```json
{
  "Result": "Nguy cơ cao",
  "RiskLevel": 78.5,
  "Recommendation": "Khuyến nghị gặp bác sĩ khám chuyên khoa ngay...",
  "Details": "Model AI dự đoán nguy cơ bệnh tim cao..."
}
```

---

## 🔗 TÍCH HỢP VỚI C# APPLICATION

### Bước 1: Cập nhật appsettings.json

File: `HealthManagement/appsettings.json`

Thêm hoặc sửa section `AISettings`:
```json
{
  "AISettings": {
    "IsEnabled": true,
    "ApiUrl": "http://localhost:5000/predict",
    "TimeoutSeconds": 30
  }
}
```

### Bước 2: Cập nhật AIService.cs

File: `HealthManagement/Services/AIService.cs`

Uncomment phần gọi Flask API trong method `PredictHealthRiskAsync`:

```csharp
public async Task<PredictionResponse> PredictHealthRiskAsync(PredictionRequest request)
{
    bool isAIEnabled = _configuration.GetValue<bool>("AISettings:IsEnabled");

    if (isAIEnabled)
    {
        // GỌI FLASK API (uncomment dòng này)
        return await CallFlaskAPIAsync(request);
    }

    // Fallback: Giả lập
    return await Task.FromResult(GenerateMockPrediction(request));
}
```

Và implement method `CallFlaskAPIAsync`:

```csharp
private async Task<PredictionResponse> CallFlaskAPIAsync(PredictionRequest request)
{
    try
    {
        var apiUrl = _configuration["AISettings:ApiUrl"];
        
        var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
        return result ?? GenerateMockPrediction(request);
    }
    catch (Exception ex)
    {
        // Log error và fallback sang mock
        Console.WriteLine($"AI API Error: {ex.Message}");
        return GenerateMockPrediction(request);
    }
}
```

### Bước 3: Restart Application

```powershell
# Stop server hiện tại (Ctrl+C)

# Rebuild
cd HealthManagement
dotnet build

# Run lại
dotnet run
```

### Bước 4: Test End-to-End

1. **Chạy Flask API:**
   ```powershell
   cd AIModel
   python flask_api.py
   ```

2. **Chạy HealthManagement:**
   ```powershell
   cd HealthManagement
   dotnet run
   ```

3. **Test trên web:**
   - Login vào web
   - Vào `/Prediction/Predict`
   - Nhập dữ liệu
   - Submit → Kết quả từ AI model

---

## 🎯 LUỒNG HOẠT ĐỘNG HOÀN CHỈNH

```
┌─────────────────┐
│   User Input    │
│  (Web Form)     │
└────────┬────────┘
         │
         ▼
┌─────────────────────┐
│ PredictionController│
│    (C# - ASP.NET)   │
└──────────┬──────────┘
           │
           ▼
┌──────────────────────┐
│     AIService.cs     │
│  CallFlaskAPIAsync   │
└──────────┬───────────┘
           │ HTTP POST
           ▼
┌────────────────────────┐
│    Flask API Server    │
│   (Python - Port 5000) │
└──────────┬─────────────┘
           │
           ▼
┌──────────────────────────┐
│   Load .pkl Models       │
│ - heart_disease_model    │
│ - scaler                 │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│  ML Prediction           │
│  - Preprocess input      │
│  - Scale features        │
│  - model.predict()       │
│  - Calculate probability │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│   Return JSON Response   │
│  {Result, RiskLevel,...} │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│  Display to User         │
│  (PredictionResult.cshtml)│
└──────────────────────────┘
```

---

## 📝 TROUBLESHOOTING

### ❌ Lỗi: "No module named 'sklearn'"
```powershell
pip install scikit-learn
```

### ❌ Lỗi: "Cannot find heart.csv"
- Kiểm tra file có trong folder `AIModel/data/` không
- Đảm bảo tên file đúng (có thể là `heart.csv` hoặc `heart_disease.csv`)

### ❌ Lỗi: Flask không chạy được
```powershell
# Kiểm tra port 5000 có bị chiếm không
netstat -ano | findstr :5000

# Đổi port khác trong flask_api.py:
app.run(host='0.0.0.0', port=5001, debug=True)
```

### ❌ Lỗi: C# không connect được Flask API
- Kiểm tra Flask server đang chạy
- Kiểm tra firewall có block port 5000 không
- Test API bằng browser: http://localhost:5000/health

---

## 🎓 GIẢI THÍCH THUẬT TOÁN

### **Logistic Regression**
- Thuật toán đơn giản, nhanh
- Tốt cho binary classification (0 hoặc 1)
- Accuracy: ~82-85%

### **Random Forest** ⭐ (Thường tốt nhất)
- Ensemble nhiều Decision Trees
- Chống overfitting tốt
- Accuracy: ~87-91%
- Có thể xem feature importance

### **Gradient Boosting**
- Kỹ thuật boosting mạnh mẽ
- Accuracy cao nhưng train lâu hơn
- Accuracy: ~86-90%

### **SVM (Support Vector Machine)**
- Tìm hyperplane phân tách tốt nhất
- Accuracy: ~84-88%

---

## 📚 TÀI LIỆU THAM KHẢO

- [Scikit-learn Documentation](https://scikit-learn.org/)
- [Flask Documentation](https://flask.palletsprojects.com/)
- [Kaggle Heart Disease Dataset](https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset)
- [UCI Machine Learning Repository](https://archive.ics.uci.edu/ml/)

---

## ✅ CHECKLIST HOÀN THÀNH

- [ ] Python 3.8+ đã cài
- [ ] Packages đã cài (`pip install -r requirements.txt`)
- [ ] Dataset đã download vào `AIModel/data/`
- [ ] Chạy `train_heart_disease.py` thành công
- [ ] Files `.pkl` đã được tạo
- [ ] Flask API chạy được (`python flask_api.py`)
- [ ] Test API bằng Postman/PowerShell thành công
- [ ] `appsettings.json` đã cập nhật
- [ ] `AIService.cs` đã uncomment CallFlaskAPIAsync
- [ ] Test end-to-end từ web thành công

---

🎉 **CHÚC MỪNG! Bạn đã hoàn thành việc train và deploy AI model!**
