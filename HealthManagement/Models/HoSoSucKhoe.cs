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
        [Display(Name = "Chiều cao (cm)")]
        public decimal ChieuCao { get; set; } // cm

        [Range(10, 300, ErrorMessage = "Cân nặng phải từ 10-300 kg")]
        [Display(Name = "Cân nặng (kg)")]
        public decimal CanNang { get; set; } // kg

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
