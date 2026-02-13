using System.ComponentModel.DataAnnotations;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng bài viết về sức khỏe (do Admin quản lý)
    /// </summary>
    public class BaiVietSucKhoe
    {
        [Key]
        public int MaBaiViet { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(300)]
        public string TieuDe { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? MoTaNgan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string NoiDung { get; set; } = string.Empty;

        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [StringLength(100)]
        public string? DanhMuc { get; set; } // Dinh dưỡng, Tập luyện, Sức khỏe tinh thần, etc

        [StringLength(500)]
        public string? TacGia { get; set; }

        public int LuotXem { get; set; } = 0;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true; // Published or Draft

        [StringLength(200)]
        public string? Tags { get; set; } // Comma separated tags
    }
}
