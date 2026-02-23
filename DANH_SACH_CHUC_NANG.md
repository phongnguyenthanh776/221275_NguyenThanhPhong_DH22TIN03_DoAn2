# DANH SÁCH CHỨC NĂNG

## USER

1. Đăng ký tài khoản (/Account/Register)
2. Đăng nhập (/Account/Login)
3. Quên mật khẩu (/Account/ForgotPassword)
4. Đặt lại mật khẩu (/Account/ResetPassword)
5. Quản lý tài khoản (/Account/Manage)
6. Dashboard tổng quan (/Home/Dashboard)
7. Hồ sơ sức khỏe (/Health/Profile)
8. Nhập chỉ số sức khỏe (/Health/AddMetric)
9. Lịch sử chỉ số (/Health/MetricHistory)
10. Tính BMI (/Health/CalculateBMI)
11. Lịch sử BMI (/Health/BMIHistory)
12. Chọn bệnh dự đoán (/Prediction/SelectDisease)
13. Dự đoán bệnh tim (/Prediction/PredictHeartDisease)
14. Dự đoán tiểu đường (/Prediction/PredictDiabetes)
15. Dự đoán huyết áp (/Prediction/PredictHypertension)
16. Dự đoán đột quỵ (/Prediction/PredictStroke)
17. Lịch sử dự đoán (/Prediction/History)
18. Chi tiết dự đoán (/Prediction/Details/{id})
19. Theo dõi giấc ngủ (/Sleep/Index)
20. Theo dõi uống nước (/Hydration/Index)
21. Quản lý thuốc (/Medication/Index)
22. Báo cáo xu hướng (/Analytics/Dashboard)
23. Báo cáo tuần (/Analytics/WeeklyReport)

## ADMIN

24. Admin dashboard (/Admin/Index)
25. Quản lý người dùng (/Admin/Users)

## GHI CHÚ

- Nhắc uống nước/thuốc có 2 trạng thái: đã gửi email và đã uống.
- AI dự đoán qua Flask API, hỗ trợ 4 bệnh.
- Email SMTP hỗ trợ quên mật khẩu và nhắc nhở.
