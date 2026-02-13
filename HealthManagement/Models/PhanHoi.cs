using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng phản hồi/góp ý từ người dùng
    /// </summary>
    public class PhanHoi
    {
        [Key]
        public int MaPhanHoi { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [StringLength(2000)]
        public string NoiDung { get; set; } = string.Empty;

        [StringLength(50)]
        public string LoaiPhanHoi { get; set; } = "Góp ý"; // Góp ý, Báo lỗi, Khác

        [Range(1, 5)]
        public int? DanhGia { get; set; } // 1-5 sao

        [Required]
        public DateTime NgayGui { get; set; } = DateTime.Now;

        public bool DaXuLy { get; set; } = false;

        [StringLength(2000)]
        public string? TraLoi { get; set; }

        public DateTime? NgayTraLoi { get; set; }

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
