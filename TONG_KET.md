# TỔNG KẾT DỰ ÁN

## TÓM TẮT

- Tên dự án: Health Management System
- Mục tiêu: Quản lý và theo dõi sức khỏe cá nhân, dự đoán nguy cơ bằng AI, nhắc nhở thông minh
- Công nghệ: ASP.NET Core MVC 8, EF Core, SQL Server, Flask AI API

## YÊU CẦU ĐÃ ĐẠT

- CRUD đầy đủ cho các module sức khỏe
- Authentication + Authorization
- Dashboard và báo cáo
- AI dự đoán 4 bệnh
- Nhắc nhở qua email và thông báo trên trình duyệt
- Giao diện responsive

## ĐIỂM NỔI BẬT

- Hồ sơ sức khỏe và chỉ số sức khỏe liên thông
- AI dự đoán nhiều bệnh qua Flask API
- Tự động gửi email nhắc uống nước/thuốc
- Quên mật khẩu qua email
- Phân tích xu hướng 30 ngày và báo cáo tuần

## CƠ SỞ DỮ LIỆU

- 11 bảng chính: NguoiDung, VaiTro, HoSoSucKhoe, ChiSoSucKhoe, LichSuBMI, DuDoanAI, GiacNgu, UongNuoc, NhacUongNuoc, Thuoc, LichUongThuoc
- Identity tables được quản lý bởi ASP.NET Identity

## LƯU Ý

- AI là công cụ hỗ trợ, không thay thế chẩn đoán y khoa.
- Email SMTP cần App Password để gửi email.

## HƯỚNG PHÁT TRIỂN

- Mở rộng thêm bệnh và dữ liệu hướng dẫn
- Tích hợp thống kê nâng cao
- Tự động nhắc nhở theo thói quen
