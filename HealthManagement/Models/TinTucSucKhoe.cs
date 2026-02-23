using System.ComponentModel.DataAnnotations;

namespace HealthManagement.Models
{
    /// <summary>
    /// Tin tức và bài viết sức khỏe
    /// Quản lý bởi Admin để hiển thị trên trang chủ
    /// </summary>
    public class TinTucSucKhoe
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [StringLength(500)]
        public string MoTa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục không được để trống")]
        [StringLength(50)]
        public string DanhMuc { get; set; } = string.Empty; // AI, Tim mạch, Dinh dưỡng, Thể dục, Huyết áp, Lối sống

        [StringLength(50)]
        public string MauDanhMuc { get; set; } = "primary"; // primary, danger, success, warning, info, secondary

        [Required(ErrorMessage = "URL hình ảnh không được để trống")]
        [StringLength(500)]
        public string HinhAnhUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string LinkBaiViet { get; set; } = "#"; // Link đến bài viết đầy đủ

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool HienThi { get; set; } = true; // Hiển thị trên trang chủ hay không

        public int ThuTu { get; set; } = 0; // Thứ tự hiển thị
    }
}
