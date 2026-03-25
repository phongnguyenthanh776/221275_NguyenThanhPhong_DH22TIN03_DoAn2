# Health Management System

**ðŸ”— LiÃªn káº¿t Há»‡ thá»‘ng Online (ÄÃ£ triá»ƒn khai trÃªn Azure)**
- **Website Frontend (C# ASP.NET Core):** [https://healthmgmt-221275-web.azurewebsites.net](https://healthmgmt-221275-web.azurewebsites.net)
- **AI Backend API (Python Flask):** [https://healthmgmt-221275-ai-final.azurewebsites.net](https://healthmgmt-221275-ai-final.azurewebsites.net)

---

## ThÃ´ng tin dá»± Ã¡n

- Sinh viÃªn: Nguyá»…n Thanh Phong
- MSSV: 221275
- Lá»›p: DH22TIN03
- MÃ´n: Äá»“ Ã¡n 2

## Tá»•ng quan

Health Management System lÃ  á»©ng dá»¥ng web quáº£n lÃ½ sá»©c khá»e cÃ¡ nhÃ¢n, phÃ¡t triá»ƒn báº±ng ASP.NET Core MVC vÃ  tÃ­ch há»£p AI qua Flask API. Há»‡ thá»‘ng há»— trá»£ theo dÃµi dá»¯ liá»‡u sá»©c khá»e, phÃ¢n tÃ­ch xu hÆ°á»›ng, nháº¯c viá»‡c qua email vÃ  dá»± Ä‘oÃ¡n nguy cÆ¡ bá»‡nh tá»« cáº£ dá»¯ liá»‡u chá»‰ sá»‘ láº«n áº£nh y táº¿.

## CÃ´ng nghá»‡ sá»­ dá»¥ng

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- SQL Server
- ASP.NET Identity
- Bootstrap 5, Chart.js
- Python Flask cho AI API
- scikit-learn / XGBoost / LightGBM cho mÃ´ hÃ¬nh tabular
- PyTorch (ResNet-18) cho mÃ´ hÃ¬nh áº£nh

## CÆ¡ sá»Ÿ dá»¯ liá»‡u

Há»‡ thá»‘ng dÃ¹ng 12 báº£ng nghiá»‡p vá»¥ chÃ­nh:

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

LÆ°u Ã½: CÃ¡c báº£ng Identity (AspNetUsers, AspNetRoles, ...) Ä‘Æ°á»£c quáº£n lÃ½ riÃªng bá»Ÿi ASP.NET Identity.

## Chá»©c nÄƒng chÃ­nh

### NgÆ°á»i dÃ¹ng

- ÄÄƒng kÃ½, Ä‘Äƒng nháº­p, quÃªn máº­t kháº©u, Ä‘áº·t láº¡i máº­t kháº©u
- Quáº£n lÃ½ tÃ i khoáº£n cÃ¡ nhÃ¢n (há» tÃªn, email, sá»‘ Ä‘iá»‡n thoáº¡i, Ä‘á»•i máº­t kháº©u)
- Dashboard tá»•ng quan sá»©c khá»e
- Há»“ sÆ¡ sá»©c khá»e vÃ  nháº­p chá»‰ sá»‘ Ä‘á»‹nh ká»³
- TÃ­nh BMI, xem lá»‹ch sá»­ BMI
- Dá»± Ä‘oÃ¡n AI cho 6 bá»‡nh vÃ  xem lá»‹ch sá»­ dá»± Ä‘oÃ¡n
- Theo dÃµi giáº¥c ngá»§
- Theo dÃµi uá»‘ng nÆ°á»›c vÃ  nháº¯c uá»‘ng nÆ°á»›c
- Quáº£n lÃ½ thuá»‘c vÃ  lá»‹ch uá»‘ng thuá»‘c
- Dashboard phÃ¢n tÃ­ch xu hÆ°á»›ng + bÃ¡o cÃ¡o tuáº§n

### Quáº£n trá»‹ viÃªn

- Dashboard quáº£n trá»‹
- Quáº£n lÃ½ ngÆ°á»i dÃ¹ng (vÃ´ hiá»‡u hÃ³a/kÃ­ch hoáº¡t)
- Quáº£n lÃ½ tin tá»©c sá»©c khá»e (táº¡o, sá»­a, xÃ³a, thá»© tá»± hiá»ƒn thá»‹)

## AI hiá»‡n táº¡i

Há»‡ thá»‘ng há»— trá»£ 6 bá»‡nh:

### Dá»¯ liá»‡u tabular (4 bá»‡nh)

- Heart Disease
- Diabetes
- Hypertension
- Stroke

Äáº·c Ä‘iá»ƒm pipeline:

- So sÃ¡nh Ä‘a mÃ´ hÃ¬nh (Logistic Regression, Random Forest, Gradient Boosting, XGBoost, LightGBM)
- Xá»­ lÃ½ máº¥t cÃ¢n báº±ng dá»¯ liá»‡u (SMOTE/biáº¿n thá»ƒ theo tá»«ng bÃ i toÃ¡n)
- Hiá»‡u chá»‰nh xÃ¡c suáº¥t
- Tá»‘i Æ°u ngÆ°á»¡ng quyáº¿t Ä‘á»‹nh thay vÃ¬ cá»‘ Ä‘á»‹nh 0.5

### Dá»¯ liá»‡u áº£nh (2 bá»‡nh)

- Kidney Stone (áº£nh CT)
- Pneumonia (áº£nh X-quang)

Äáº·c Ä‘iá»ƒm pipeline:

- ResNet-18 pretrained (PyTorch)
- Tráº£ vá» xÃ¡c suáº¥t, nhÃ£n dá»± Ä‘oÃ¡n, ngÆ°á»¡ng quyáº¿t Ä‘á»‹nh
- Tráº£ vá» áº£nh gá»‘c + áº£nh chÃº thÃ­ch Grad-CAM

LÆ°u Ã½ ká»¹ thuáº­t:

- Lá»‹ch sá»­ dá»± Ä‘oÃ¡n áº£nh lÆ°u metadata JSON trong trÆ°á»ng `DuLieuDauVao`
- áº¢nh dá»± Ä‘oÃ¡n Ä‘Æ°á»£c lÆ°u táº¡i `wwwroot/uploads/predictions/...`

## HÆ°á»›ng dáº«n cháº¡y

### 1. YÃªu cáº§u

- .NET SDK 8
- SQL Server
- Python 3.9+

### 2. Cáº¥u hÃ¬nh á»©ng dá»¥ng web

- Má»Ÿ `HealthManagement/appsettings.json`
- Kiá»ƒm tra `ConnectionStrings:DefaultConnection`
- Kiá»ƒm tra `AISettings`:

```json
"AISettings": {
  "ApiUrl": "http://localhost:5000/predict",
  "IsEnabled": true,
  "TimeoutSeconds": 30
}
```

### 3. Cáº­p nháº­t cÆ¡ sá»Ÿ dá»¯ liá»‡u

```bash
dotnet ef database update --project HealthManagement/HealthManagement.csproj
```

### 4. CÃ i mÃ´i trÆ°á»ng AI (náº¿u cáº§n cháº¡y Flask)

```bash
python -m venv .venv
.venv\Scripts\activate
pip install -r AIModel/requirements.txt
```

### 5. Cháº¡y Flask AI API

```bash
cd AIModel
python flask_api.py
```

Kiá»ƒm tra health endpoint:

```bash
GET http://localhost:5000/health
```

### 6. Cháº¡y á»©ng dá»¥ng ASP.NET Core

```bash
dotnet run --project HealthManagement/HealthManagement.csproj
```

Máº·c Ä‘á»‹nh truy cáº­p táº¡i: `https://localhost:5001` hoáº·c URL do terminal cung cáº¥p.

### 7. Cáº¥u hÃ¬nh email nháº¯c viá»‡c (tÃ¹y chá»n)

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

## TÃ i liá»‡u AI chi tiáº¿t

Xem thÃªm: `AIModel/README_AI.md`

## LÆ°u Ã½

- ÄÃ¢y lÃ  cÃ´ng cá»¥ há»— trá»£ theo dÃµi sá»©c khá»e, khÃ´ng thay tháº¿ cháº©n Ä‘oÃ¡n chuyÃªn mÃ´n.
- Náº¿u test quÃªn máº­t kháº©u hoáº·c nháº¯c viá»‡c email, cáº§n kiá»ƒm tra cáº£ há»™p thÆ° Spam/Quáº£ng cÃ¡o.

## LiÃªn há»‡

- Email: phong221275@student.nctu.edu.vn


