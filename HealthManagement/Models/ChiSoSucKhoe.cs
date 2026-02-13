using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng chỉ số sức khỏe: huyết áp, nhịp tim, đường huyết, cholesterol
    /// </summary>
    public class ChiSoSucKhoe
    {
        [Key]
        public int MaChiSo { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Range(0, 300)]
        public int? HuyetApTam { get; set; } // mmHg (Systolic)

        [Range(0, 200)]
        public int? HuyetApThan { get; set; } // mmHg (Diastolic)

        [Range(30, 250)]
        public int? NhipTim { get; set; } // bpm

        [Range(0, 500)]
        public decimal? DuongHuyet { get; set; } // mg/dL

        [Range(0, 500)]
        public decimal? Cholesterol { get; set; } // mg/dL

        [Range(0, 100)]
        public decimal? ChiSoOxy { get; set; } // SpO2 %

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        public DateTime NgayDo { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
