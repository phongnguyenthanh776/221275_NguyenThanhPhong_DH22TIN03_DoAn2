# Health Management System

## Thông tin dự án

- Sinh viên: Nguyễn Thanh Phong
- MSSV: 221275
- Lớp: DH22TIN03
- Môn: Đồ án 2

## Mục tiêu

Xây dựng hệ thống quản lý và theo dõi sức khỏe cá nhân với các tính năng:
- Hồ sơ sức khỏe, chỉ số sức khỏe, BMI
- Dự đoán nguy cơ bằng AI cho 4 bệnh (tim, tiểu đường, huyết áp, đột quỵ)
- Quản lý lối sống: giấc ngủ, uống nước, thuốc
- Báo cáo phân tích, xu hướng, cảnh báo
- Nhắc nhở qua email và thông báo trên trình duyệt
- Quản trị người dùng
- Quên mật khẩu và đặt lại mật khẩu

## Công nghệ

- ASP.NET Core MVC 8
- Entity Framework Core (Code First)
- SQL Server
- ASP.NET Identity
- Bootstrap 5, Chart.js
- Python + Flask (AI API)

## Cấu trúc cơ sở dữ liệu (12 bảng chính)

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

Lưu ý: Bảng Identity từ ASP.NET Identity được quản lý riêng. Bảng TinTucSucKhoe chứa tin tức sức khỏe được Admin quản lý hiển thị trên trang chủ.

## Chức năng chính

### Người dùng
- Đăng ký, đăng nhập, quản lý tài khoản
- Quên mật khẩu, đặt lại mật khẩu qua email
- Dashboard tổng quan
- Hồ sơ sức khỏe, nhập chỉ số, lịch sử chỉ số
- Tính BMI và lịch sử BMI
- Dự đoán AI 4 bệnh + lịch sử dự đoán
- Theo dõi giấc ngủ
- Theo dõi uống nước và nhắc uống nước
- Quản lý thuốc và nhắc uống thuốc
- Báo cáo tuần, phân tích xu hướng

### Admin
- Dashboard quản trị
- Quản lý người dùng (kích hoạt/vô hiệu hóa)
- Quản lý tin tức sức khỏe (tạo, sửa, xóa, hiển thị/ẩn)

## Hướng dẫn chạy

### Yêu cầu
- .NET SDK 8
- SQL Server
- Python 3.9+ (nếu chạy AI API)

### Bước 1: Coi kết nối DB
- Mở appsettings.json
- Kiểm tra ConnectionStrings:DefaultConnection

### Bước 2: Cập nhật DB
```
dotnet ef database update
```

### Bước 3: Chạy ứng dụng
```
dotnet run --project HealthManagement/HealthManagement.csproj
```

### Bước 4: Chạy AI API (tùy chọn)
```
cd AIModel
python flask_api.py
```

### Bước 5: Cấu hình email
Trong appsettings.json:
```
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

## AI hiện tại

- Flask API tại http://localhost:5000
- Các file model được load theo mẫu:
  - heart_disease_model.pkl
  - diabetes_model.pkl
  - hypertension_model.pkl
  - stroke_model.pkl
- AIService (C#) gọi Flask API qua AISettings:ApiUrl

## Lưu ý

- Hệ thống là công cụ hỗ trợ, không thay thế chẩn đoán y khoa.
- Nên kiểm tra mail trong cả Spam/Quảng cáo khi test quên mật khẩu.

## Liên hệ

- Email: phong221275@student.nctu.edu.vn
