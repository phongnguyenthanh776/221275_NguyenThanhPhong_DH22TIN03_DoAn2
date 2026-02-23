using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Nhac nho uong nuoc theo gio.
    /// </summary>
    [Table("NhacUongNuoc")]
    public class NhacUongNuoc
    {
        [Key]
        public int MaNhac { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Display(Name = "Gio nhac")]
        [DataType(DataType.DateTime)]
        public DateTime GioNhac { get; set; }

        [Display(Name = "So ml")]
        [Range(50, 5000)]
        public int? SoMl { get; set; }

        [Display(Name = "Da uong")]
        public bool DaUong { get; set; } = false;

        [Display(Name = "Da gui mail")]
        public bool DaGuiEmail { get; set; } = false;

        [Display(Name = "Ghi chu")]
        [StringLength(200)]
        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
