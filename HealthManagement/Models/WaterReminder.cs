using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Nhắc nhở uống nước theo giờ.
    /// </summary>
    [Table("NhacUongNuoc")]
    public class WaterReminder
    {
        [Key]
        public int MaNhac { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Display(Name = "Giờ nhắc")]
        [DataType(DataType.DateTime)]
        public DateTime GioNhac { get; set; }

        [Display(Name = "Số ml")]
        [Range(50, 5000)]
        public int? SoMl { get; set; }

        [Display(Name = "Đã uống")]
        public bool DaUong { get; set; } = false;

        [Display(Name = "Đã gửi mail")]
        public bool DaGuiEmail { get; set; } = false;

        [Display(Name = "Ghi chú")]
        [StringLength(200)]
        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
