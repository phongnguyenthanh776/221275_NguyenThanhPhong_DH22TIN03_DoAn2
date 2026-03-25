# Health Management System

**🔗 Liên kết Hệ thống Online (Đã triển khai trên Azure)**
- **Website Frontend (C# ASP.NET Core):** [https://healthmgmt-221275-web.azurewebsites.net](https://healthmgmt-221275-web.azurewebsites.net)
- **AI Backend API (Python Flask):** [https://healthmgmt-221275-ai-final.azurewebsites.net](https://healthmgmt-221275-ai-final.azurewebsites.net)

---

## Thông tin dự án

- Sinh viên: Nguyễn Thanh Phong
- MSSV: 221275
- Lớp: DH22TIN03
- Môn: Đồ án 2

## Tổng quan

Health Management System là ứng dụng web quản lý sức khỏe cá nhân, phát triển bằng ASP.NET Core MVC và tích hợp AI qua Flask API. Hệ thống hỗ trợ theo dõi dữ liệu sức khỏe, phân tích xu hướng, nhắc việc qua email và dự đoán nguy cơ bệnh từ cả dữ liệu chỉ số lẫn ảnh y tế.

## Công nghệ sử dụng

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- SQL Server
- ASP.NET Identity
- Bootstrap 5, Chart.js
- Python Flask cho AI API
- scikit-learn / XGBoost / LightGBM cho mô hình tabular
- PyTorch (ResNet-18) cho mô hình ảnh

## Cơ sở dữ liệu

Hệ thống dùng 12 bảng nghiệp vụ chính:

1. VaiTro
2. NguoiDung
3. HoSoSucKhoe
4. ChiSoSucKhoe
5. LichSuBMI
6. DuDoanAI
7. GiacNgu
8. UongNuoc
9. NhacUongNuoc
10. Thuoc
11. LichUongThuoc
12. TinTucSucKhoe

Lưu ý: Các bảng Identity (AspNetUsers, AspNetRoles, ...) được quản lý riêng bởi ASP.NET Identity.

## Chức năng chính

### Người dùng

- Đăng ký, đăng nhập, quên mật khẩu, đặt lại mật khẩu
- Quản lý tài khoản cá nhân (họ tên, email, số điện thoại, đổi mật khẩu)
- Dashboard tổng quan sức khỏe
- Hồ sơ sức khỏe và nhập chỉ số định kỳ
- Tính BMI, xem lịch sử BMI
- Dự đoán AI cho 6 bệnh và xem lịch sử dự đoán
- Theo dõi giấc ngủ
- Theo dõi uống nước và nhắc uống nước
- Quản lý thuốc và lịch uống thuốc
- Dashboard phân tích xu hướng + báo cáo tuần

### Quản trị viên

- Dashboard quản trị
- Quản lý người dùng (vô hiệu hóa/kích hoạt)
- Quản lý tin tức sức khỏe (tạo, sửa, xóa, thứ tự hiển thị)

## AI hiện tại

Hệ thống hỗ trợ 6 bệnh:

### Dữ liệu tabular (4 bệnh)

- Heart Disease
- Diabetes
- Hypertension
- Stroke

Đặc điểm pipeline:

- So sánh đa mô hình (Logistic Regression, Random Forest, Gradient Boosting, XGBoost, LightGBM)
- Xử lý mất cân bằng dữ liệu (SMOTE/biến thể theo từng bài toán)
- Hiệu chỉnh xác suất
- Tối ưu ngưỡng quyết định thay vì cố định 0.5

### Dữ liệu ảnh (2 bệnh)

- Kidney Stone (ảnh CT)
- Pneumonia (ảnh X-quang)

Đặc điểm pipeline:

- ResNet-18 pretrained (PyTorch)
- Trả về xác suất, nhãn dự đoán, ngưỡng quyết định
- Trả về ảnh gốc + ảnh chú thích Grad-CAM

Lưu ý kỹ thuật:

- Lịch sử dự đoán ảnh lưu metadata JSON trong trường `DuLieuDauVao`
- Ảnh dự đoán được lưu tại `wwwroot/uploads/predictions/...`

## Hướng dẫn chạy

### 1. Yêu cầu

- .NET SDK 8
- SQL Server
- Python 3.9+

### 2. Cấu hình ứng dụng web

- Mở `HealthManagement/appsettings.json`
- Kiểm tra `ConnectionStrings:DefaultConnection`
- Kiểm tra `AISettings`:

```json
"AISettings": {
  "ApiUrl": "http://localhost:5000/predict",
  "IsEnabled": true,
  "TimeoutSeconds": 30
}
```

### 3. Cập nhật cơ sở dữ liệu

```bash
dotnet ef database update --project HealthManagement/HealthManagement.csproj
```

### 4. Cài môi trường AI (nếu cần chạy Flask)

```bash
python -m venv .venv
.venv\Scripts\activate
pip install -r AIModel/requirements.txt
```

### 5. Chạy Flask AI API

```bash
cd AIModel
python flask_api.py
```

Kiểm tra health endpoint:

```bash
GET http://localhost:5000/health
```

### 6. Chạy ứng dụng ASP.NET Core

```bash
dotnet run --project HealthManagement/HealthManagement.csproj
```

Mặc định truy cập tại: `https://localhost:5001` hoặc URL do terminal cung cấp.

### 7. Cấu hình email nhắc việc (tùy chọn)

Trong `HealthManagement/appsettings.json`:

```json
"EmailSettings": {
  "Enabled": true,
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "UseSsl": true,
  "SenderEmail": "your@gmail.com",
  "SenderName": "Health Management",
  "AppPassword": "your-app-password"
}
```

## Tài liệu AI chi tiết

Xem thêm: `AIModel/README_AI.md`

## Lưu ý

- Đây là công cụ hỗ trợ theo dõi sức khỏe, không thay thế chẩn đoán chuyên môn.
- Nếu test quên mật khẩu hoặc nhắc việc email, cần kiểm tra cả hộp thư Spam/Quảng cáo.

## Liên hệ

- Email: phong221275@student.nctu.edu.vn



