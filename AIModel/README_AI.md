# AI MODEL TRAINING & API

## Mục tiêu

Hướng dẫn huấn luyện mô hình dự đoán bệnh (tabular và ảnh CNN) rồi khởi động Flask API để tích hợp với hệ thống Health Management.

Hệ thống hỗ trợ dự đoán **6 bệnh**:
- 4 bệnh dữ liệu bảng (tabular): tim mạch, tiểu đường, huyết áp cao, đột quỵ.
- 2 bệnh dựa trên ảnh CNN: sỏi thận, viêm phổi.

## Cấu trúc thư mục

```
AIModel/
├── data/               # CSV và dataset ảnh
├── training_pipeline.py        # Pipeline dùng chung cho 4 bệnh tabular
├── train_heart_disease.py
├── train_diabetes.py
├── train_hypertension.py
├── train_stroke.py
├── train_kidney_stone_image.py
├── train_pneumonia_image.py
├── image_cnn_utils.py          # Tiện ích CNN + Grad-CAM
└── flask_api.py
```

## Cài đặt môi trường

```
cd ..                      # Ra thư mục gốc dự án
python -m venv .venv
.venv\Scripts\activate
pip install -r AIModel/requirements.txt
```

## Huấn luyện mô hình

### Mô hình tabular (4 bệnh)

Chạy lần lượt 4 script từ thư mục `AIModel/`:

```
python train_heart_disease.py
python train_diabetes.py
python train_hypertension.py
python train_stroke.py
```

**Pipeline nâng cấp — công nghệ áp dụng đồng nhất cho cả 4 bệnh:**

Mỗi lần huấn luyện sẽ so sánh đồng thời tối đa 5 thuật toán: Logistic Regression, Random Forest, Gradient Boosting, XGBoost (nếu có) và LightGBM (nếu có). Pipeline tự động xếp hạng mô hình theo công thức `0.5 × ROC-AUC + 0.3 × PR-AUC + 0.2 × F1` rồi chọn mô hình tốt nhất.

Khi tỷ lệ nhãn thiểu số/đa số nhỏ hơn 0.85, SMOTE tự động kích hoạt để sinh thêm mẫu tổng hợp, tránh mô hình bỏ sót ca bệnh thật. Sau khi chọn xong mô hình tốt nhất, xác suất đầu ra được hiệu chỉnh bằng `CalibratedClassifierCV` phương pháp sigmoid để phản ánh đúng khả năng thực tế. Cuối cùng, pipeline quét ngưỡng quyết định từ 0.10 đến 0.90, chọn ngưỡng tối ưu theo F1 với ràng buộc Recall tối thiểu (thay vì dùng ngưỡng mặc định 0.5 cố định).

**Hiệu năng thực tế (lần train gần nhất):**

| Bệnh | Accuracy | ROC-AUC | PR-AUC | Mô hình được chọn |
|---|---|---|---|---|
| Tim mạch | 73.8% | 89.3% | 91.0% | Logistic Regression |
| Tiểu đường | 74.7% | 81.8% | 67.7% | Random Forest |
| Huyết áp cao | 72.2% | 80.3% | 78.2% | XGBoost |
| Đột quỵ | ~78.0%* | 80.7% | 18.0% | XGBoost |

*Đột quỵ có tỷ lệ mất cân bằng nghiêm trọng (chỉ ~5% ca dương), Recall đột quỵ đạt ~64% nhờ threshold được hạ thấp còn ~0.06.

**File xuất ra sau khi train:**

```
heart_disease_model.pkl / heart_disease_scaler.pkl / heart_disease_features.pkl / heart_disease_threshold_meta.pkl
diabetes_model.pkl / diabetes_scaler.pkl / diabetes_features.pkl / diabetes_threshold_meta.pkl
hypertension_model.pkl / hypertension_scaler.pkl / hypertension_features.pkl / hypertension_threshold_meta.pkl
stroke_model.pkl / stroke_scaler.pkl / stroke_features.pkl / stroke_threshold_meta.pkl
```

Flask API đọc file `*_threshold_meta.pkl` để dùng ngưỡng tối ưu thay vì 0.5 mặc định. Response trả về các trường `Probability`, `PredictedClass`, `DecisionThreshold`, `ModelType`.

### Mô hình ảnh CNN (2 bệnh)

```
python train_kidney_stone_image.py
python train_pneumonia_image.py
```

**Kiến trúc:** PyTorch ResNet-18 pretrained (transfer learning, feature extraction mode) + Grad-CAM để giải thích vùng ảnh mô hình chú ý.

**Chiến lược dữ liệu:** Cả hai script gom **toàn bộ ảnh** từ tất cả split có sẵn (train/val/test nếu tồn tại) rồi tách lại train/validation nội bộ theo tỷ lệ cấu hình, đảm bảo không bỏ phí ảnh nào.

Có thể điều chỉnh qua biến môi trường:
```
# Viêm phổi
PNEUMONIA_IMG_EPOCHS=8        # mặc định 5
PNEUMONIA_IMG_BATCH=16
PNEUMONIA_IMG_VAL_RATIO=0.15  # mặc định 0.15

# Sỏi thận
KIDNEY_IMG_EPOCHS=8           # mặc định 5
KIDNEY_IMG_BATCH=16
KIDNEY_IMG_VAL_RATIO=0.20     # mặc định 0.20
```

**Hiệu năng thực tế (train 8 epoch, full dataset):**

| Bệnh | Tổng ảnh | Val Accuracy | Balanced Acc | F1 | AUC |
|---|---|---|---|---|---|
| Sỏi thận | 6.454 | 93.73% | 92.43% | 0.860 | 0.973 |
| Viêm phổi | 5.856 | 94.20% | 93.51% | 0.960 | 0.976 |

**File xuất ra:**

```
kidney_stone_image_model.pt / kidney_stone_image_labels.pkl / kidney_stone_image_meta.json
pneumonia_image_model.pt / pneumonia_image_labels.pkl / pneumonia_image_meta.json
```

## Khởi động Flask API

```
python flask_api.py
```

Kiểm tra nhanh:
```
GET http://localhost:5000/health
```

## Gọi API dự đoán

```
POST http://localhost:5000/predict
```

Ví dụ body JSON (bệnh tim):
```json
{
  "disease_type": "heart_disease",
  "Data": {
    "age": 55,
    "sex": "Male",
    "chestpaintype": 2,
    "restingbp": 140,
    "cholesterol": 250,
    "fastingbs": 1,
    "maxhr": 150,
    "exerciseangina": 0
  }
}
```

Các `disease_type` được hỗ trợ: `heart_disease`, `diabetes`, `hypertension`, `stroke`, `kidney_stone_image`, `pneumonia_image`.

## Tích hợp với C#

Trong `HealthManagement/appsettings.json`:

```json
"AISettings": {
  "ApiUrl": "http://localhost:5000/predict",
  "IsEnabled": true,
  "TimeoutSeconds": 30
}
```

## Lưu ý

Flask API tự động mapping tên cột và điền giá trị thiếu theo scaler. Nếu chưa có file model `.pkl`/`_threshold_meta.pkl` hoặc file `.pt`, API sẽ thông báo thiếu model. Với dự đoán bằng ảnh, Flask API trả về xác suất, nhãn dự đoán cùng ảnh Grad-CAM overlay (base64) để giao diện C# hiển thị kết quả giải thích trực quan.
