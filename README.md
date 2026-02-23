# Health Management System

## Thong tin du an

- Sinh vien: Nguyen Thanh Phong
- MSSV: 221275
- Lop: DH22TIN03
- Mon: Do an 2

## Muc tieu

Xay dung he thong quan ly va theo doi suc khoe ca nhan voi cac tinh nang:
- Ho so suc khoe, chi so suc khoe, BMI
- Du doan nguy co bang AI cho 4 benh (tim, tieu duong, huyet ap, dot quy)
- Quan ly loi song: giac ngu, uong nuoc, thuoc
- Bao cao phan tich, xu huong, canh bao
- Nhac nho qua email va thong bao tren trinh duyet
- Quan tri nguoi dung
- Quen mat khau va dat lai mat khau

## Cong nghe

- ASP.NET Core MVC 8
- Entity Framework Core (Code First)
- SQL Server
- ASP.NET Identity
- Bootstrap 5, Chart.js
- Python + Flask (AI API)

## Cau truc co so du lieu (11 bang chinh)

1. VaiTro
2. NguoiDung
3. HoSoSucKhoe
4. ChiSoSucKhoe
5. LichSuBMI
6. DuDoanAI
7. GiacNgu
8. UongNuoc
9. WaterReminder
10. Thuoc
11. LichUongThuoc

Luu y: Bang Identity tu ASP.NET Identity duoc quan ly rieng.

## Chuc nang chinh

### Nguoi dung
- Dang ky, dang nhap, quan ly tai khoan
- Quen mat khau, dat lai mat khau qua email
- Dashboard tong quan
- Ho so suc khoe, nhap chi so, lich su chi so
- Tinh BMI va lich su BMI
- Du doan AI 4 benh + lich su du doan
- Theo doi giac ngu
- Theo doi uong nuoc va nhac uong nuoc
- Quan ly thuoc va nhac uong thuoc
- Bao cao tuan, phan tich xu huong

### Admin
- Dashboard quan tri
- Quan ly nguoi dung (kich hoat/vo hieu hoa)

## Huong dan chay

### Yeu cau
- .NET SDK 8
- SQL Server
- Python 3.9+ (neu chay AI API)

### Buoc 1: Coi ket noi DB
- Mo appsettings.json
- Kiem tra ConnectionStrings:DefaultConnection

### Buoc 2: Cap nhat DB
```
dotnet ef database update
```

### Buoc 3: Chay ung dung
```
dotnet run --project HealthManagement/HealthManagement.csproj
```

### Buoc 4: Chay AI API (tuy chon)
```
cd AIModel
python flask_api.py
```

### Buoc 5: Cau hinh email
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

## AI hien tai

- Flask API tai http://localhost:5000
- Cac file model duoc load theo mau:
  - heart_disease_model.pkl
  - diabetes_model.pkl
  - hypertension_model.pkl
  - stroke_model.pkl
- AIService (C#) goi Flask API qua AISettings:ApiUrl

## Luu y

- He thong la cong cu ho tro, khong thay the chan doan y khoa.
- Nen kiem tra mail trong ca Spam/Quang cao khi test quen mat khau.

## Lien he

- Email: phong221275@student.nctu.edu.vn
