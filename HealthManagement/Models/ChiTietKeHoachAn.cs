using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng chi tiết kế hoạch ăn (nhiều món trong 1 kế hoạch)
    /// </summary>
    public class ChiTietKeHoachAn
    {
        [Key]
        public int MaChiTiet { get; set; }

        [Required]
        public int MaKeHoach { get; set; }

        [Required]
        public int MaMonAn { get; set; }

        [Required]
        public DateTime NgayAn { get; set; }

        [Required]
        [StringLength(50)]
        public string BuaAn { get; set; } = string.Empty; // Sáng, Trưa, Tối, Phụ

        [Range(0, 100)]
        public decimal KhoiLuong { get; set; } = 1; // Khẩu phần (VD: 1, 0.5, 2)

        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Navigation Properties
        [ForeignKey("MaKeHoach")]
        public virtual KeHoachAnUong? KeHoachAnUong { get; set; }

        [ForeignKey("MaMonAn")]
        public virtual ThucDon? ThucDon { get; set; }
    }
}
