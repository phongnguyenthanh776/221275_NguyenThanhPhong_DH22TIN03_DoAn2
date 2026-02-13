using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng kế hoạch ăn uống của người dùng
    /// </summary>
    public class KeHoachAnUong
    {
        [Key]
        public int MaKeHoach { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(200)]
        public string TenKeHoach { get; set; } = string.Empty;

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Range(0, 50000)]
        public decimal MucTieuCalo { get; set; } // Calo mục tiêu mỗi ngày

        [StringLength(500)]
        public string? GhiChu { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }

        public virtual ICollection<ChiTietKeHoachAn> ChiTietKeHoachAn { get; set; } = new List<ChiTietKeHoachAn>();
    }
}
