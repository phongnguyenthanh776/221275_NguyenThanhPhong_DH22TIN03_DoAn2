"""Train a CNN image classifier for pneumonia detection with Grad-CAM support."""

from pathlib import Path
import os

from sklearn.model_selection import train_test_split

try:
    from image_cnn_utils import DEFAULT_IMAGE_SIZE, train_binary_classifier
except Exception:
    from AIModel.image_cnn_utils import DEFAULT_IMAGE_SIZE, train_binary_classifier


CLASS_NAMES = ["NORMAL", "PNEUMONIA"]
SEED = 42
IMG_SIZE = DEFAULT_IMAGE_SIZE
EPOCHS = int(os.getenv("PNEUMONIA_IMG_EPOCHS", "5"))
BATCH_SIZE = int(os.getenv("PNEUMONIA_IMG_BATCH", "16"))
VAL_RATIO = float(os.getenv("PNEUMONIA_IMG_VAL_RATIO", "0.15"))

base_dir = Path(__file__).resolve().parent
root_dir = base_dir / "data" / "chest_xray"
train_dir = root_dir / "train"
val_dir = root_dir / "val"
test_dir = root_dir / "test"
model_path = base_dir / "pneumonia_image_model.pt"
labels_path = base_dir / "pneumonia_image_labels.pkl"
meta_path = base_dir / "pneumonia_image_meta.json"


def list_images(folder: Path):
    return [
        path
        for path in folder.rglob("*")
        if path.suffix.lower() in {".jpg", ".jpeg", ".png", ".bmp"}
    ]


candidate_roots = [folder for folder in [train_dir, val_dir, test_dir, root_dir] if folder.exists()]
if not candidate_roots:
    raise FileNotFoundError(f"Khong tim thay du lieu anh trong: {root_dir}")

all_samples = []
split_stats = {}
seen_paths = set()

for split_dir in candidate_roots:
    split_name = split_dir.name
    split_stats.setdefault(split_name, {})
    for label_idx, class_name in enumerate(CLASS_NAMES):
        class_dir = split_dir / class_name
        if not class_dir.exists():
            split_stats[split_name][class_name] = 0
            continue
        image_paths = list_images(class_dir)
        split_stats[split_name][class_name] = len(image_paths)
        for image_path in image_paths:
            path_key = str(image_path.resolve())
            if path_key in seen_paths:
                continue
            seen_paths.add(path_key)
            all_samples.append((image_path, label_idx))

if not all_samples:
    raise RuntimeError("Khong doc duoc anh nao de train Pneumonia CNN.")

labels = [label for _, label in all_samples]
val_ratio = min(max(float(VAL_RATIO), 0.05), 0.4)
train_samples, val_samples = train_test_split(
    all_samples,
    test_size=val_ratio,
    random_state=SEED,
    stratify=labels,
)

print("=" * 70)
print("PNEUMONIA IMAGE TRAINING (CNN + GRAD-CAM)")
print("=" * 70)
print(f"Dataset root: {root_dir}")
print("Using all available images from train/val/test splits when present")
for split_name, counts in split_stats.items():
    print(f"- {split_name}: NORMAL={counts.get('NORMAL', 0)}, PNEUMONIA={counts.get('PNEUMONIA', 0)}")
print(f"Total unique images used: {len(all_samples)}")
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
    disease_type="pneumonia_image",
    image_size=IMG_SIZE,
    epochs=EPOCHS,
    batch_size=BATCH_SIZE,
    val_source="internal_split_from_all_images",
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
