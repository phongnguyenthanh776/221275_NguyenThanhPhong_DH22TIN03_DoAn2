"""Train a CNN image classifier for kidney stone detection with Grad-CAM support."""

from pathlib import Path
import os

from sklearn.model_selection import train_test_split

try:
    from image_cnn_utils import DEFAULT_IMAGE_SIZE, train_binary_classifier
except Exception:
    from AIModel.image_cnn_utils import DEFAULT_IMAGE_SIZE, train_binary_classifier


CLASS_NAMES = ["Normal", "Stone"]
SEED = 42
IMG_SIZE = DEFAULT_IMAGE_SIZE
EPOCHS = int(os.getenv("KIDNEY_IMG_EPOCHS", "5"))
BATCH_SIZE = int(os.getenv("KIDNEY_IMG_BATCH", "16"))
VAL_RATIO = float(os.getenv("KIDNEY_IMG_VAL_RATIO", "0.2"))

base_dir = Path(__file__).resolve().parent
source_dir = base_dir / "data" / "CT-KIDNEY-DATASET-Normal-Cyst-Tumor-Stone" / "CT-KIDNEY-DATASET-Normal-Cyst-Tumor-Stone"
model_path = base_dir / "kidney_stone_image_model.pt"
labels_path = base_dir / "kidney_stone_image_labels.pkl"
meta_path = base_dir / "kidney_stone_image_meta.json"


def list_images(folder: Path):
    return [
        path
        for path in folder.rglob("*")
        if path.suffix.lower() in {".jpg", ".jpeg", ".png", ".bmp"}
    ]


if not source_dir.exists():
    raise FileNotFoundError(f"Khong tim thay thu muc dataset: {source_dir}")

normal_dir = source_dir / "Normal"
stone_dir = source_dir / "Stone"
if not normal_dir.exists() or not stone_dir.exists():
    raise FileNotFoundError("Dataset CT than thieu class Normal hoac Stone.")

samples = [(path, 0) for path in list_images(normal_dir)]
samples.extend((path, 1) for path in list_images(stone_dir))
if not samples:
    raise RuntimeError("Khong doc duoc anh nao de train Kidney Stone CNN.")

labels = [label for _, label in samples]
val_ratio = min(max(float(VAL_RATIO), 0.05), 0.4)
train_samples, val_samples = train_test_split(
    samples,
    test_size=val_ratio,
    random_state=SEED,
    stratify=labels,
)

print("=" * 70)
print("KIDNEY STONE IMAGE TRAINING (CNN + GRAD-CAM)")
print("=" * 70)
print(f"Dataset: {source_dir}")
print("Using all available images in Normal/Stone classes")
print(f"Total images used: {len(samples)}")
print(f"Validation ratio: {val_ratio:.2f}")
print(f"Train size: {len(train_samples)}")
print(f"Validation size: {len(val_samples)}")

meta = train_binary_classifier(
    train_samples,
    val_samples,
    CLASS_NAMES,
    model_path,
    labels_path,
    meta_path,
    disease_type="kidney_stone_image",
    image_size=IMG_SIZE,
    epochs=EPOCHS,
    batch_size=BATCH_SIZE,
)

print("=" * 70)
print("DONE")
print(f"Model type: {meta['model_type']}")
print(f"Labels: {labels_path}")
print(f"Meta: {meta_path}")
print(
    f"Val accuracy: {meta['val_accuracy']:.4f}, "
    f"BAcc: {meta['val_balanced_accuracy']:.4f}, "
    f"F1: {meta['val_f1']:.4f}, AUC: {meta['val_auc']:.4f}"
)
print(f"Decision threshold: {meta['threshold']:.3f}")
print("=" * 70)
