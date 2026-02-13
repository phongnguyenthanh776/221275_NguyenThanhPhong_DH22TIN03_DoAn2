# 🏥 Health Management System - Hệ thống Quản lý Sức khỏe

## 📋 Thông tin dự án

**Sinh viên:** Nguyễn Thanh Phong  
**MSSV:** 221275  
**Lớp:** DH22TIN03  
**Môn:** Đồ án 2

## 🎯 Mục tiêu

Xây dựng Web Application quản lý và theo dõi sức khỏe cá nhân thông minh với khả năng:
- Quản lý thông tin sức khỏe cá nhân
- Theo dõi các chỉ số: BMI, huyết áp, nhịp tim, đường huyết, cholesterol
- Dự đoán nguy cơ sức khỏe bằng AI (sẵn sàng tích hợp Machine Learning)
- Quản lý thực đơn lành mạnh
- Cung cấp kiến thức sức khỏe qua bài viết

## 🛠️ Công nghệ sử dụng

### Backend
- **ASP.NET Core MVC 8.0**
- **Entity Framework Core** (Code First)
- **SQL Server** (LocalDB)
- **ASP.NET Identity** (Authentication & Authorization)

### Frontend
- **Bootstrap 5.3** (Responsive UI)
- **Chart.js 4.4** (Biểu đồ)
- **Bootstrap Icons**
- **jQuery**

### Chuẩn bị cho AI (Chưa triển khai)
- **Python** (scikit-learn, Flask)
- **Kaggle Dataset**: Heart Disease / Diabetes
- **ML Models**: Logistic Regression, Random Forest

## 📊 Cấu trúc Database (12 bảng)

1. **VaiTro** - Vai trò người dùng
2. **NguoiDung** - Thông tin người dùng
3. **HoSoSucKhoe** - Hồ sơ sức khỏe chi tiết
4. **ChiSoSucKhoe** - Các chỉ số: huyết áp, nhịp tim, đường huyết...
5. **LichSuBMI** - Lịch sử tính BMI
6. **ThucDon** - Danh sách món ăn
7. **KeHoachAnUong** - Kế hoạch ăn uống của người dùng
8. **ChiTietKeHoachAn** - Chi tiết món ăn trong kế hoạch
9. **DuDoanAI** - Lưu kết quả dự đoán AI
10. **LichSuChatBot** - Lịch sử chat (placeholder)
11. **BaiVietSucKhoe** - Bài viết về sức khỏe
12. **PhanHoi** - Phản hồi từ người dùng

## 🎨 Các chức năng chính (≥14 Forms)

### Người dùng
1. ✅ **Đăng ký tài khoản**
2. ✅ **Đăng nhập**
3. ✅ **Dashboard** - Tổng quan sức khỏe
4. ✅ **Cập nhật hồ sơ sức khỏe**
5. ✅ **Nhập chỉ số sức khỏe**
6. ✅ **Xem lịch sử chỉ số**
7. ✅ **Tính BMI**
8. ✅ **Xem lịch sử BMI**
9. ✅ **Dự đoán nguy cơ AI** (hiện tại giả lập)
10. ✅ **Xem lịch sử dự đoán**
11. ✅ **Xem thực đơn**
12. ✅ **Xem bài viết sức khỏe**

### Admin
13. ✅ **Quản lý người dùng**
14. ✅ **Quản lý bài viết** (Tạo, Sửa, Xóa)
15. ✅ **Quản lý thực đơn** (Tạo, Sửa, Xóa)

## 🚀 Hướng dẫn cài đặt

### Yêu cầu
- Visual Studio 2022 hoặc mới hơn
- .NET 8.0 SDK
- SQL Server (LocalDB)

### Các bước

1. **Clone hoặc mở project**
   ```bash
   cd HealthManagement
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Cập nhật Connection String** (nếu cần)
   - Mở `appsettings.json`
   - Kiểm tra `ConnectionStrings:DefaultConnection`

4. **Tạo Database**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```

6. **Truy cập**
   - URL: `https://localhost:5001`
   - Admin: `admin@health.com` / `Admin@123`

## 📁 Cấu trúc Project

```
HealthManagement/
├── Controllers/           # Controllers (MVC)
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── HealthController.cs
│   ├── PredictionController.cs
│   ├── MealController.cs
│   ├── ArticleController.cs
│   ├── AdminController.cs
│   └── API/
│       └── PredictController.cs
├── Models/                # Models (12 bảng)
│   ├── NguoiDung.cs
│   ├── VaiTro.cs
│   ├── HoSoSucKhoe.cs
│   ├── ChiSoSucKhoe.cs
│   ├── LichSuBMI.cs
│   ├── ThucDon.cs
│   ├── KeHoachAnUong.cs
│   ├── ChiTietKeHoachAn.cs
│   ├── DuDoanAI.cs
│   ├── LichSuChatBot.cs
│   ├── BaiVietSucKhoe.cs
│   └── PhanHoi.cs
├── Services/              # Business Logic
│   ├── AIService.cs       (Giả lập dự đoán)
│   ├── HealthService.cs
│   ├── MealService.cs
│   ├── ArticleService.cs
│   └── UserService.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs        (Dữ liệu mẫu)
├── Views/                 # Razor Views
│   ├── Shared/
│   ├── Home/
│   ├── Account/
│   ├── Health/
│   ├── Prediction/
│   ├── Meal/
│   ├── Article/
│   └── Admin/
└── appsettings.json      # Cấu hình
```

## 🤖 Về tính năng AI (Chưa triển khai)

### Hiện tại
- Hệ thống sử dụng **thuật toán giả lập** dựa trên các ngưỡng y khoa cơ bản
- API endpoint `/api/predict` đã sẵn sàng
- Database đã có bảng `DuDoanAI` để lưu kết quả

### Kế hoạch triển khai AI

#### Bước 1: Thu thập Dataset
- Download từ Kaggle:
  - [Heart Disease Dataset](https://www.kaggle.com/datasets/johnsmith88/heart-disease-dataset)
  - [Diabetes Dataset](https://www.kaggle.com/datasets/mathchi/diabetes-data-set)

#### Bước 2: Train Model (Python)
```python
# Sử dụng scikit-learn
from sklearn.linear_model import LogisticRegression
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

# Train và so sánh models
# Lưu model tốt nhất: joblib.dump(model, 'model.pkl')
```

#### Bước 3: Tạo Flask API
```python
# Flask API để ASP.NET gọi
from flask import Flask, request, jsonify
import joblib

app = Flask(__name__)
model = joblib.load('model.pkl')

@app.route('/predict', methods=['POST'])
def predict():
    data = request.json
    prediction = model.predict([data])
    return jsonify({'result': prediction})
```

#### Bước 4: Tích hợp vào ASP.NET
- Cập nhật `appsettings.json`:
  ```json
  "AISettings": {
    "ApiUrl": "http://localhost:5000/predict",
    "IsEnabled": true
  }
  ```
- Code trong `AIService.cs` đã sẵn sàng, chỉ cần uncomment phần `CallFlaskAPIAsync()`

## 📝 Lưu ý quan trọng

### ⚠️ Disclaimer
- Hệ thống này là công cụ **HỖ TRỢ**, KHÔNG THAY THẾ chẩn đoán y khoa
- Người dùng cần tham khảo ý kiến bác sĩ chuyên khoa
- Dự đoán AI hiện tại chỉ mang tính chất tham khảo

### 🔒 Bảo mật
- Sử dụng ASP.NET Identity cho Authentication
- Password: Hash + Salt
- Authorization: Role-based (Admin, NguoiDung)
- HTTPS required

### 🎨 UI/UX
- Responsive design (Bootstrap 5)
- Mobile-friendly
- Modern & Clean interface
- Chart.js cho visualize data

## 📚 Tài liệu tham khảo

- ASP.NET Core MVC: https://docs.microsoft.com/aspnet/core/mvc
- Entity Framework Core: https://docs.microsoft.com/ef/core
- Bootstrap 5: https://getbootstrap.com
- Chart.js: https://www.chartjs.org
- Kaggle Datasets: https://www.kaggle.com/datasets

## 🎓 Về Đồ án

### Mục tiêu học tập
- Áp dụng ASP.NET Core MVC
- Thiết kế Database với EF Core
- Xây dựng RESTful API
- Chuẩn bị hạ tầng cho Machine Learning
- Phát triển giao diện responsive

### Phạm vi
- ✅ Hoàn thiện toàn bộ CRUD operations
- ✅ Authentication & Authorization
- ✅ Dashboard với biểu đồ
- ✅ Cấu trúc sẵn sàng cho AI
- ⏳ Train ML model (giai đoạn sau)

## 📧 Liên hệ

**Sinh viên:** Nguyễn Thanh Phong  
**Email:** phong221275@student.nctu.edu.vn 
**Lớp:** DH22TIN03

---

⭐ **Lưu ý:** Project này được xây dựng cho mục đích học tập. 
Để triển khai thực tế, cần có sự tham gia của chuyên gia y tế và tuân thủ các quy định về dữ liệu sức khỏe.
