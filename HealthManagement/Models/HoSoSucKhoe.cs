using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng hồ sơ sức khỏe chi tiết của người dùng
    /// </summary>
    public class HoSoSucKhoe
    {
        [Key]
        public int MaHoSo { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Range(30, 250, ErrorMessage = "Chiều cao phải từ 30-250 cm")]
        public decimal ChieuCao { get; set; } // cm

        [Range(10, 300, ErrorMessage = "Cân nặng phải từ 10-300 kg")]
        public decimal CanNang { get; set; } // kg

        [StringLength(20)]
        public string? NhomMau { get; set; } // A, B, AB, O

        [StringLength(500)]
        public string? TienSuBenhLy { get; set; }

        [StringLength(500)]
        public string? DiUng { get; set; }

        [StringLength(500)]
        public string? ThuocDangDung { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
