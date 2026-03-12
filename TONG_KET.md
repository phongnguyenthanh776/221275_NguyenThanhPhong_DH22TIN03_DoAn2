# TỔNG KẾT DỰ ÁN

## TÓM TẮT

- Tên dự án: Health Management System
- Mục tiêu: Quản lý và theo dõi sức khỏe cá nhân, dự đoán nguy cơ bệnh bằng AI, nhắc nhở thông minh
- Công nghệ: ASP.NET Core MVC 8, EF Core, SQL Server, Flask AI API (Python), PyTorch CNN, scikit-learn

## YÊU CẦU ĐÃ ĐẠT

- CRUD đầy đủ cho các module sức khỏe
- Authentication + Authorization (ASP.NET Identity)
- Dashboard và báo cáo xu hướng 30 ngày
- AI dự đoán 6 bệnh (4 tabular + 2 ảnh CNN)
- Nhắc nhở qua email và thông báo trên trình duyệt
- Giao diện responsive

## ĐIỂM NỔI BẬT

- Hồ sơ sức khỏe và chỉ số sức khỏe liên thông
- AI dự đoán 6 bệnh qua Flask API: tim mạch, tiểu đường, huyết áp cao, đột quỵ (tabular) và sỏi thận, viêm phổi (ảnh CNN ResNet-18 + Grad-CAM)
- Pipeline tabular nâng cấp: so sánh 5 thuật toán (LR, RF, GB, XGBoost, LightGBM), SMOTE tự động, calibration xác suất sigmoid, tối ưu ngưỡng quyết định
- Dự đoán ảnh trả về kết quả kèm ảnh Grad-CAM overlay giải thích vùng mô hình chú ý
- Tự động gửi email nhắc uống nước và uống thuốc
- Quên mật khẩu qua email token
- Phân tích xu hướng 30 ngày và báo cáo tuần

## HIỆU NĂNG MÔ HÌNH AI (lần train gần nhất)

| Bệnh | Loại | Accuracy | ROC-AUC | F1 |
|---|---|---|---|---|
| Tim mạch | Tabular | 73.8% | 89.3% | 0.79 |
| Tiểu đường | Tabular | 74.7% | 81.8% | 0.61 |
| Huyết áp cao | Tabular | 72.2% | 80.3% | 0.74 |
| Đột quỵ | Tabular | ~78%* | 80.7% | 0.23* |
| Sỏi thận | CNN ảnh | 93.7% | 97.3% | 0.86 |
| Viêm phổi | CNN ảnh | 94.2% | 97.6% | 0.96 |

*Đột quỵ: Recall ca bệnh thật đạt ~64% nhờ threshold tối ưu; F1 thấp do mất cân bằng dữ liệu nghiêm trọng (~5% dương).

## CƠ SỞ DỮ LIỆU

- 11 bảng chính: NguoiDung, VaiTro, HoSoSucKhoe, ChiSoSucKhoe, LichSuBMI, DuDoanAI, GiacNgu, UongNuoc, NhacUongNuoc, Thuoc, LichUongThuoc
- Thêm bảng TinTucSucKhoe cho nội dung quản trị
- Identity tables được quản lý bởi ASP.NET Identity

## LƯU Ý

- AI là công cụ hỗ trợ, không thay thế chẩn đoán y khoa.
- Email SMTP cần App Password để gửi email.
- Cần chạy Flask API riêng trước khi sử dụng tính năng dự đoán AI.

## HƯỚNG PHÁT TRIỂN

- Mở rộng thêm bệnh (ví dụ: ung thư da từ ảnh dermoscopy)
- Fine-tuning toàn bộ backbone CNN (unfrozen pretrained layers) khi có GPU
- Tích hợp thống kê nâng cao và gợi ý cá nhân hóa
- Tự động nhắc nhở theo thói quen người dùng
