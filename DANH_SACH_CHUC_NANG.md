# 📊 DANH SÁCH CHỨC NĂNG & FORM

## ✅ HOÀN THÀNH: 14+ FORMS

### 👤 NGƯỜI DÙNG (User Features)

#### 1. **Đăng ký tài khoản** (`/Account/Register`)
- Form nhập: Họ tên, Email, Số điện thoại, Ngày sinh, Giới tính, Mật khẩu
- Validation đầy đủ
- Tự động tạo vai trò NguoiDung

#### 2. **Đăng nhập** (`/Account/Login`)
- Form: Email, Password, Remember Me
- ASP.NET Identity Authentication
- Admin demo: admin@health.com / Admin@123

#### 3. **Dashboard** (`/Home/Dashboard`)
- Hiển thị thống kê: BMI, Huyết áp, Nhịp tim, Số lần dự đoán
- Biểu đồ BMI (Chart.js)
- Biểu đồ Huyết áp (Chart.js)
- Lịch sử dự đoán gần nhất
- Quick action buttons

#### 4. **Hồ sơ sức khỏe** (`/Health/Profile`)
- Form nhập: Chiều cao, Cân nặng, Nhóm máu
- Tiền sử bệnh lý, Dị ứng, Thuốc đang dùng
- Tự động cập nhật theo người dùng

#### 5. **Nhập chỉ số sức khỏe** (`/Health/AddMetric`)
- Form nhập 6 chỉ số:
  - Huyết áp tâm thu/trương (mmHg)
  - Nhịp tim (bpm)
  - Chỉ số Oxy SpO2 (%)
  - Đường huyết (mg/dL)
  - Cholesterol (mg/dL)
- Có ghi chú riêng

#### 6. **Lịch sử chỉ số** (`/Health/MetricHistory`)
- Bảng hiển thị lịch sử
- Có thể xem 20 bản ghi gần nhất

#### 7. **Tính BMI** (`/Health/CalculateBMI`)
- Form nhập: Chiều cao (cm), Cân nặng (kg)
- Tự động tính toán BMI
- Phân loại: Thiếu cân, Bình thường, Thừa cân, Béo phì
- Đưa ra lời khuyên cụ thể

#### 8. **Lịch sử BMI** (`/Health/BMIHistory`)
- Biểu đồ line chart thể hiện biến động BMI
- Bảng chi tiết từng lần đo
- Màu sắc theo phân loại

#### 9. **Dự đoán nguy cơ AI** (`/Prediction/Predict`)
- Form nhập 8 thông số:
  - Tuổi, Giới tính
  - Loại đau ngực (0-3)
  - Huyết áp nghỉ
  - Cholesterol
  - Đường huyết lúc đói
  - Nhịp tim tối đa
  - Đau thắt ngực khi vận động
- Hiện tại: Giả lập bằng rule-based
- Sẵn sàng: Tích hợp ML model từ Kaggle

#### 10. **Kết quả dự đoán** (`/Prediction/PredictionResult`)
- Hiển thị: Nguy cơ thấp/trung bình/cao
- Mức độ nguy cơ (%)
- Progress bar màu sắc
- Chi tiết và lời khuyên

#### 11. **Lịch sử dự đoán** (`/Prediction/History`)
- Danh sách tất cả lần dự đoán
- Card layout với màu theo mức độ
- Chi tiết từng dự đoán

#### 12. **Xem thực đơn** (`/Meal/Index`)
- Filter theo loại: Sáng, Trưa, Tối, Phụ
- Hiển thị: Tên món, Calo, Protein, Carbs, Fat
- Card layout đẹp mắt

#### 13. **Xem bài viết** (`/Article/Index`)
- Tìm kiếm bài viết
- Filter theo danh mục, tags
- Đếm lượt xem
- Card layout responsive

---

### 🔐 ADMIN (Admin Features)

#### 14. **Admin Dashboard** (`/Admin/Index`)
- Thống kê tổng quan:
  - Số người dùng
  - Số bài viết
  - Số món ăn
- Quick actions

#### 15. **Quản lý người dùng** (`/Admin/Users`)
- Xem danh sách user
- Vô hiệu hóa user
- Hiển thị vai trò, trạng thái

#### 16. **Quản lý bài viết** (`/Admin/Articles`)
- Danh sách bài viết
- Tạo mới / Chỉnh sửa / Xóa
- Đếm lượt xem

#### 17. **Tạo/Sửa bài viết** (`/Admin/CreateArticle`, `/Admin/EditArticle`)
- Form đầy đủ:
  - Tiêu đề
  - Danh mục (dropdown)
  - Mô tả ngắn
  - Nội dung (hỗ trợ HTML)
  - Tags
  - Hình ảnh (URL)
  - Trạng thái (Published/Draft)

#### 18. **Quản lý thực đơn** (`/Admin/Meals`)
- Danh sách món ăn
- Tạo mới / Chỉnh sửa / Xóa

#### 19. **Tạo/Sửa món ăn** (`/Admin/CreateMeal`, `/Admin/EditMeal`)
- Form nhập:
  - Tên món ăn
  - Mô tả
  - Calo, Protein, Carbs, Fat
  - Loại món (Sáng/Trưa/Tối/Phụ)
  - Hình ảnh

---

## 📊 DATABASE: 12 BẢNG

1. ✅ **VaiTro** - Roles (Admin, NguoiDung)
2. ✅ **NguoiDung** - User information + Identity link
3. ✅ **HoSoSucKhoe** - Health profile (1-1 với NguoiDung)
4. ✅ **ChiSoSucKhoe** - Health metrics (1-n với NguoiDung)
5. ✅ **LichSuBMI** - BMI history (1-n với NguoiDung)
6. ✅ **ThucDon** - Meal database
7. ✅ **KeHoachAnUong** - Meal plans (1-n với NguoiDung)
8. ✅ **ChiTietKeHoachAn** - Meal plan details (n-n relationship)
9. ✅ **DuDoanAI** - AI prediction results (1-n với NguoiDung)
10. ✅ **LichSuChatBot** - Chatbot history (placeholder)
11. ✅ **BaiVietSucKhoe** - Health articles
12. ✅ **PhanHoi** - User feedback (1-n với NguoiDung)

---

## 🎨 UI/UX FEATURES

✅ **Bootstrap 5** - Responsive design  
✅ **Bootstrap Icons** - Icon set  
✅ **Chart.js** - BMI & BP charts trên Dashboard  
✅ **Gradient colors** - Modern UI  
✅ **Card hover effects** - Smooth animations  
✅ **Progress bars** - Visual risk levels  
✅ **Badge system** - Status indicators  
✅ **Alert notifications** - Success/Error messages  
✅ **Mobile responsive** - Works on all devices  
✅ **Dark header** - Gradient navbar  

---

## 🔒 SECURITY & AUTHORIZATION

✅ **ASP.NET Identity** - Built-in authentication  
✅ **Role-based authorization** - Admin vs NguoiDung  
✅ **Password hashing** - Secure storage  
✅ **[Authorize] attributes** - Route protection  
✅ **HTTPS** - Secure connections  
✅ **Anti-forgery tokens** - CSRF protection  
✅ **Model validation** - Server-side validation  

---

## 🤖 AI PREPARATION (Ready to integrate)

✅ **AIService interface** - Abstraction layer  
✅ **Mock prediction** - Rule-based fallback  
✅ **DuDoanAI table** - Store predictions  
✅ **API endpoint** - `/api/predict` ready  
✅ **DTO models** - PredictionRequest/Response  
✅ **Configuration** - appsettings.json AI settings  

**Chỉ cần:**
1. Train model từ Kaggle dataset
2. Tạo Flask API
3. Cập nhật `AISettings:IsEnabled = true`
4. Uncomment code trong `AIService.CallFlaskAPIAsync()`

---

## 📈 CHARTS & VISUALIZATION

✅ **Dashboard**: 2 biểu đồ line charts  
- BMI theo thời gian (7 ngày)  
- Huyết áp tâm thu/trương (7 ngày)  

✅ **BMI History**: Line chart đầy đủ  
✅ **Color-coded badges**: Risk levels  
✅ **Progress bars**: Visual percentage  

---

## ✅ TỔNG KẾT

**Forms:** 19+ forms hoàn chỉnh  
**Database:** 12 bảng với Foreign Keys  
**UI:** Bootstrap 5 + Chart.js  
**Backend:** ASP.NET Core MVC 8 + EF Core  
**Security:** Identity + Authorization  
**AI:** Sẵn sàng tích hợp  

🎯 **ĐẠT YÊU CẦU:** ≥10 forms, ≥12 bảng, Giao diện đẹp, Chuẩn bị AI ✅
