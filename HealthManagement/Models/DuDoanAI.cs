using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Enum các loại bệnh hỗ trợ dự đoán
    /// </summary>
    public enum DiseaseType
    {
        HeartDisease,    // Bệnh Tim
        Diabetes,        // Bệnh Tiểu Đường
        Hypertension,    // Huyết Áp Cao
        Stroke           // Đột Quỵ
    }

    /// <summary>
    /// Bảng lưu kết quả dự đoán từ AI
    /// Hỗ trợ dự đoán 4 loại bệnh với AI Models thực (không phải rule-based)
    /// Models: Heart Disease, Diabetes, Hypertension, Stroke
    /// </summary>
    public class DuDoanAI
    {
        [Key]
        public int MaDuDoan { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        public DiseaseType LoaiBenhDuDoan { get; set; } = DiseaseType.HeartDisease;

        [Required]
        [StringLength(100)]
        public string KetQua { get; set; } = string.Empty; // Nguy cơ thấp, trung bình, cao

        [Range(0, 100)]
        public decimal MucDoNguyCo { get; set; } // % (0-100)

        [StringLength(1000)]
        public string? ChiTietDuDoan { get; set; }

        [StringLength(1000)]
        public string? GoiY { get; set; }

        [Required]
        public DateTime NgayDuDoan { get; set; } = DateTime.Now;

        [StringLength(2000)]
        public string? DuLieuDauVao { get; set; } // JSON format

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }

    // ============================================================
    // GENERIC REQUEST/RESPONSE DTOs
    // ============================================================

    /// <summary>
    /// Generic prediction request - hỗ trợ nhiều loại bệnh
    /// </summary>
    public class MultiDiseaseRequest
    {
        [Required]
        public DiseaseType DiseaseType { get; set; } = DiseaseType.HeartDisease;

        [Required]
        public Dictionary<string, object> Data { get; set; } = new();
    }

    /// <summary>
    /// Generic prediction response
    /// </summary>
    public class PredictionResponse
    {
        public string? DiseaseType { get; set; }
        public string Result { get; set; } = string.Empty;
        public decimal RiskLevel { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    // ============================================================
    // SPECIFIC REQUEST DTOs (cho mỗi loại bệnh)
    // ============================================================

    /// <summary>
    /// Heart Disease Prediction Request (8 features)
    /// </summary>
    public class HeartDiseaseRequest
    {
        [Required]
        [Range(20, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Sex { get; set; } = "Male"; // Male hoặc Female

        [Range(0, 3)]
        public int? ChestPainType { get; set; } // 0-3

        [Range(80, 200)]
        public int? RestingBP { get; set; } // Huyết áp tâm thu

        [Range(0, 400)]
        public decimal? Cholesterol { get; set; }

        [Range(0, 200)]
        public decimal? FastingBS { get; set; } // Đường huyết lúc đói

        [Range(60, 220)]
        public int? MaxHR { get; set; } // Nhịp tim tối đa

        [Range(0, 1)]
        public int? ExerciseAngina { get; set; } // Đau thắt ngực khi vận động
    }

    /// <summary>
    /// Diabetes Prediction Request (8 features)
    /// </summary>
    public class DiabetesRequest
    {
        [Range(0, 20)]
        public int? Pregnancies { get; set; }

        [Range(0, 200)]
        public float? Glucose { get; set; }

        [Range(0, 122)]
        public float? BloodPressure { get; set; }

        [Range(0, 100)]
        public float? SkinThickness { get; set; }

        [Range(0, 900)]
        public float? Insulin { get; set; }

        [Range(10, 70)]
        public float? BMI { get; set; }

        [Range(0, 3)]
        public float? DiabetesPedigreeFunction { get; set; }

        [Range(20, 100)]
        public int? Age { get; set; }
    }

    /// <summary>
    /// Hypertension Prediction Request (10 features)
    /// </summary>
    public class HypertensionRequest
    {
        [Range(20, 100)]
        public int? Age { get; set; }

        [Range(0, 1)]
        public int? Gender { get; set; } // 0 = Female, 1 = Male

        [Range(10, 80)]
        public float? BMI { get; set; }

        [Range(100, 400)]
        public int? Cholesterol { get; set; }

        [Range(80, 200)]
        public int? SystolicBP { get; set; }

        [Range(40, 130)]
        public int? DiastolicBP { get; set; }

        [Range(40, 200)]
        public int? HeartRate { get; set; }

        [Range(0, 1)]
        public int? Smoking { get; set; }

        [Range(0, 1)]
        public int? Alcohol { get; set; }

        [Range(0, 200)]
        public int? PhysicalActivity { get; set; }
    }

    /// <summary>
    /// Stroke Prediction Request (8 features)
    /// </summary>
    public class StrokeRequest
    {
        [Range(20, 100)]
        public int? Age { get; set; }

        [Range(0, 1)]
        public int? Gender { get; set; } // 0 = Female, 1 = Male

        [Range(0, 1)]
        public int? Hypertension { get; set; }

        [Range(0, 1)]
        public int? HeartDisease { get; set; }

        [Range(0, 1)]
        public int? Smoking { get; set; }

        [Range(10, 80)]
        public float? BMI { get; set; }

        [Range(0, 300)]
        public float? AvgBloodPressure { get; set; }

        [Range(50, 400)]
        public float? Glucose { get; set; }
    }

    // ============================================================
    // LEGACY CLASS (compatibility)
    // ============================================================

    /// <summary>
    /// Legacy: Heart Disease only (for backward compatibility)
    /// </summary>
    public class PredictionRequest
    {
        public int Age { get; set; }
        public string Sex { get; set; } = string.Empty;
        public int? ChestPainType { get; set; }
        public int? RestingBP { get; set; }
        public decimal? Cholesterol { get; set; }
        public decimal? FastingBS { get; set; }
        public int? MaxHR { get; set; }
        public int? ExerciseAngina { get; set; }
    }
}
