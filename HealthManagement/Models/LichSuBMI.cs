using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng lịch sử BMI của người dùng
    /// </summary>
    public class LichSuBMI
    {
        [Key]
        public int MaLichSu { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Range(30, 250)]
        public decimal ChieuCao { get; set; } // cm

        [Required]
        [Range(10, 300)]
        public decimal CanNang { get; set; } // kg

        [Required]
        public decimal BMI { get; set; }

        [Required]
        [StringLength(50)]
        public string PhanLoai { get; set; } = string.Empty; // Thiếu cân, Bình thường, Thừa cân, Béo phì

        [StringLength(500)]
        public string? GoiY { get; set; }

        [Required]
        public DateTime NgayTinh { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
