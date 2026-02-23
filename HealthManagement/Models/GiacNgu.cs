using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Nhật ký giấc ngủ của người dùng
    /// </summary>
    public class GiacNgu
    {
        [Key]
        public int MaGiacNgu { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Display(Name = "Giờ ngủ")]
        [DataType(DataType.DateTime)]
        public DateTime GioNgu { get; set; }

        [Required]
        [Display(Name = "Giờ dậy")]
        [DataType(DataType.DateTime)]
        public DateTime GioDay { get; set; }

        [Display(Name = "Tổng giờ")]
        [Range(0, 24)]
        public decimal TongGio { get; set; }

        [Display(Name = "Chất lượng")]
        [StringLength(50)]
        public string? ChatLuong { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(300)]
        public string? GhiChu { get; set; }

        public DateTime NgayGhi { get; set; } = DateTime.Now;

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
