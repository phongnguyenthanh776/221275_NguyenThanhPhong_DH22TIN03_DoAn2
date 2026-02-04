# 🏥 Hệ Thống Quản Lý Sức Khỏe Thông Minh

## 📋 Mô Tả Dự Án
Ứng dụng web quản lý sức khỏe cá nhân với tính năng phân tích AI, gợi ý thực đơn, và theo dõi các chỉ số sức khỏe.

---

## ✨ Các Tính Năng Chính

### 1. 📊 Tổng Quan Sức Khỏe
- Hiển thị các chỉ số gần nhất: BMI, Cân nặng, Huyết áp, Đường huyết
- Biểu đồ xu hướng 7 ngày (Cân nặng)
- Số lượng cảnh báo chưa xem

### 2. 👤 Quản Lý Thông Tin Cá Nhân
- Cập nhật: Họ tên, Tuổi, Giới tính, Chiều cao, Cân nặng
- Lưu trữ thông tin vào database

### 3. 📈 Chỉ Số Sức Khỏe
- Nhập: Cân nặng, Huyết áp, Nhịp tim, Đường huyết
- Lịch sử chỉ số (bảng)
- Tính toán tự động các chỉ số nguy cơ

### 4. 🔢 Tính Toán BMI
- Công thức: BMI = Cân nặng (kg) / Chiều cao (m)²
- 7 mức độ phân loại BMI (Gầy độ III → Béo phì độ III)
- Lịch sử BMI với biểu đồ

### 5. 🏃 Thói Quen Sinh Hoạt
- Số giờ ngủ trung bình/ngày
- Thời gian tập luyện (phút/ngày)
- Hút thuốc, uống rượu/bia

### 6. 🍎 Gợi Ý Thực Đơn
- Gợi ý tự động dựa trên BMI
- Danh sách 50+ món ăn lành mạnh
- Chi tiết: Calo, Đạm, Béo, Bột hydrat

### 7. ⚠️ Cảnh Báo Sức Khỏe
- Cảnh báo tự động khi:
  - BMI ngoài bình thường
  - Huyết áp cao
  - Đường huyết cao
- Trạng thái: Chưa xem / Đã xem

### 8. 🤖 Phân Tích AI (Rule-Based)
- Phân tích tổng hợp:
  - Đánh giá tổng thể sức khỏe
  - Các vấn đề phát hiện
  - Khuyến nghị cải thiện
  - Mục tiêu 3 tháng
- Lịch sử phân tích
- **Không phụ thuộc API bên ngoài, chạy 100% trên server**

---

## 🔧 Công Nghệ Sử Dụng

### Backend
- **C# ASP.NET Core** - REST API
- **SQL Server** - Database
- **Entity Framework Core** - ORM
- **Google Generative AI (Gemini)** - Optional (hiện dùng Rule-Based)

### Frontend
- **HTML5, CSS3, Bootstrap 5** - Giao diện
- **JavaScript (Vanilla)** - Xử lý logic
- **Chart.js** - Biểu đồ dữ liệu

---

## 📦 Cơ Sở Dữ Liệu

### Các Bảng Chính
1. **NguoiDung** - Thông tin người dùng (tên đăng nhập, mật khẩu)
2. **ThongTinCaNhan** - Họ tên, tuổi, giới tính, chiều cao, cân nặng
3. **ChiSoSucKhoe** - Cân nặng, huyết áp, nhịp tim, đường huyết (theo ngày)
4. **LichSuBMI** - Lịch sử tính BMI
5. **ThoiQuenSinhHoat** - Giờ ngủ, thời gian tập luyện, hút thuốc, uống rượu
6. **MonAn** - Danh sách 50+ món ăn lành mạnh
7. **CanhBaoSucKhoe** - Cảnh báo tự động
8. **GoiYSucKhoeAI** - Lịch sử phân tích AI

---

## 🚀 Hướng Dẫn Chạy

### Backend
```bash
cd backend
dotnet restore
dotnet build
dotnet run
# API chạy trên: http://localhost:5000
```

### Frontend
```bash
# Mở file: frontend/index.html trong trình duyệt
# Hoặc sử dụng Live Server (VSCode)
```

### Database
```sql
-- Import script tạo database (nếu có)
-- SQL Server Connection String:
-- Server=localhost;Database=QuanLySucKhoeThongMinh;
-- User Id=sa;Password=123456;TrustServerCertificate=True;
```

---

## 📱 Quy Trình Sử Dụng

1. **Đăng ký / Đăng nhập** → Tạo tài khoản
2. **Cập nhật thông tin cá nhân** → Nhập chiều cao, cân nặng
3. **Nhập chỉ số sức khỏe** → Hàng ngày/tuần
4. **Xem tổng quan** → Dashboard với biểu đồ
5. **Nhận cảnh báo** → Khi có dấu hiệu bất thường
6. **Xem gợi ý thực đơn** → Món ăn lành mạnh theo tình trạng
7. **Phân tích AI** → Nhận tư vấn chi tiết từ AI Rule-Based

---

## 🔐 Bảo Mật

- Mật khẩu mã hóa (hash) trong database
- Session authentication
- CORS enabled cho frontend
- Validation trên backend

---

## 📊 Công Thức AI Rule-Based

### 1. Tính Nguy Cơ Tiểu Đường
```
Điểm = (BMI × 30) + (Đường huyết × 0.4) + (Tuổi × 0.2) + (Tiền sử × 10)
Nguy cơ (%) = Điểm / 100
```

### 2. Tính Nguy Cơ Cao Huyết Áp
```
Điểm = (Tâm thu × 0.4) + (Tâm trương × 0.2) + (BMI × 0.2) + (Tuổi × 0.1) + (Thói quen × 0.1)
Nguy cơ (%) = Điểm / 100
```

### 3. Phân Loại BMI
- **Gầy độ III**: BMI < 16
- **Gầy độ II**: 16 ≤ BMI < 17
- **Gầy độ I**: 17 ≤ BMI < 18.5
- **Bình thường**: 18.5 ≤ BMI < 25
- **Thừa cân**: 25 ≤ BMI < 30
- **Béo phì độ I**: 30 ≤ BMI < 35
- **Béo phì độ II**: 35 ≤ BMI < 40
- **Béo phì độ III**: BMI ≥ 40

---

## 🎯 Tính Năng Sắp Tới

- [ ] Integrasi Machine Learning (ML) cho dự đoán chính xác hơn
- [ ] Gọi API Gemini nâng cao (khi quota đủ)
- [ ] Báo cáo PDF
- [ ] Mobile app
- [ ] Đồng bộ dữ liệu cloud

---

## 👨‍💻 Tác Giả
**Nguyễn Thanh Phong** - DH22TIN03 - Đồ Án 2

---

## 📝 Ghi Chú
- Hệ thống hiện dùng **AI Rule-Based** (không phụ thuộc API bên ngoài)
- Có thể bật Gemini API khi cần (xem `AIController.cs`)
- Tất cả dữ liệu được lưu trữ an toàn trong SQL Server
