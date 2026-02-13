using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng lưu kết quả dự đoán từ AI
    /// CHÚ Ý: Hiện tại dữ liệu giả lập, sau khi train model từ Kaggle sẽ lưu kết quả thực
    /// Dataset: Heart Disease hoặc Diabetes từ Kaggle
    /// Model: Logistic Regression hoặc Random Forest
    /// </summary>
    public class DuDoanAI
    {
        [Key]
        public int MaDuDoan { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string LoaiDuDoan { get; set; } = string.Empty; // Heart Disease, Diabetes, etc

        [Required]
        [StringLength(100)]
        public string KetQua { get; set; } = string.Empty; // Nguy cơ thấp, trung bình, cao

        [Range(0, 100)]
        public decimal MucDoNguyCo { get; set; } // % (0-100)

        [StringLength(1000)]
        public string? ChiTietDuDoan { get; set; } // JSON hoặc text mô tả chi tiết

        [StringLength(1000)]
        public string? GoiY { get; set; } // Lời khuyên

        [Required]
        public DateTime NgayDuDoan { get; set; } = DateTime.Now;

        // Lưu các chỉ số đầu vào để tham khảo
        [StringLength(2000)]
        public string? DuLieuDauVao { get; set; } // JSON format

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }

    /// <summary>
    /// DTO cho request dự đoán
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

    /// <summary>
    /// DTO cho response dự đoán
    /// </summary>
    public class PredictionResponse
    {
        public string Result { get; set; } = string.Empty;
        public decimal RiskLevel { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
