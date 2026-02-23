using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Thuốc người dùng đang sử dụng.
    /// </summary>
    public class Thuoc
    {
        [Key]
        public int MaThuoc { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Display(Name = "Tên thuốc")]
        [StringLength(120)]
        public string Ten { get; set; } = string.Empty;

        [Display(Name = "Liều dùng")]
        [StringLength(120)]
        public string? LieuDung { get; set; }

        [Display(Name = "Đơn vị")]
        [StringLength(50)]
        public string? DonVi { get; set; }

        [Display(Name = "Số lần/ngày")]
        [Range(0, 24)]
        public int? SoLanNgay { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(300)]
        public string? GhiChu { get; set; }

        [Display(Name = "Còn dùng")]
        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }

        public virtual ICollection<LichUongThuoc> LichUongThuoc { get; set; } = new List<LichUongThuoc>();
    }
}
