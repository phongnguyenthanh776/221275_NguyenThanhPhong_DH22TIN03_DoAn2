# DANH SÁCH CHỨC NĂNG

## USER

1. Đăng ký tài khoản (/Account/Register)
2. Đăng nhập (/Account/Login)
3. Quên mật khẩu (/Account/ForgotPassword)
4. Đặt lại mật khẩu (/Account/ResetPassword)
5. Quản lý tài khoản (/Account/Manage)
6. Dashboard tổng quan (/Home/Dashboard)
7. Xem tin tức sức khỏe trên trang chủ (/Home/Index)
8. Hồ sơ sức khỏe (/Health/Profile)
9. Nhập chỉ số sức khỏe (/Health/AddMetric)
10. Lịch sử chỉ số (/Health/MetricHistory)
11. Tính BMI (/Health/CalculateBMI)
12. Lịch sử BMI (/Health/BMIHistory)
13. Chọn bệnh dự đoán (/Prediction/SelectDisease)
14. Dự đoán bệnh tim (/Prediction/PredictHeartDisease)
15. Dự đoán tiểu đường (/Prediction/PredictDiabetes)
16. Dự đoán huyết áp (/Prediction/PredictHypertension)
17. Dự đoán đột quỵ (/Prediction/PredictStroke)
18. Lịch sử dự đoán (/Prediction/History)
19. Chi tiết dự đoán (/Prediction/Details/{id})
20. Theo dõi giấc ngủ (/Sleep/Index)
21. Theo dõi uống nước (/Hydration/Index)
22. Quản lý thuốc (/Medication/Index)
23. Báo cáo xu hướng (/Analytics/Dashboard)
24. Báo cáo tuần (/Analytics/WeeklyReport)

## ADMIN

25. Admin dashboard (/Admin/Index)
26. Quản lý người dùng (/Admin/Users)
27. Danh sách tin tức (/Admin/TinTuc)
28. Tạo tin tức mới (/Admin/TaoTinTuc)
29. Chỉnh sửa tin tức (/Admin/SuaTinTuc/{id})
30. Xóa tin tức (/Admin/XoaTinTuc/{id})

## GHI CHÚ

- Nhắc uống nước/thuốc có 2 trạng thái: đã gửi email và đã uống.
- AI dự đoán qua Flask API, hỗ trợ 4 bệnh.
- Email SMTP hỗ trợ quên mật khẩu và nhắc nhở.
- Tin tức sức khỏe được Admin quản lý, hiển thị trên trang chủ theo thứ tự ưu tiên (số nhỏ hiển thị trước).
- Mỗi tin tức có các trường: tiêu đề, mô tả, danh mục, URL hình ảnh, link bài viết, thứ tự hiển thị, trạng thái hiển thị.
