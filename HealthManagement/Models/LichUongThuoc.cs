using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Lịch nhắc uống thuốc cho từng thuốc.
    /// </summary>
    public class LichUongThuoc
    {
        [Key]
        public int MaLichUong { get; set; }

        [Required]
        public int MaThuoc { get; set; }

        [Required]
        [Display(Name = "Giờ nhắc")]
        [DataType(DataType.DateTime)]
        public DateTime GioNhac { get; set; }

        [Display(Name = "Đã uống")]
        public bool DaUong { get; set; } = false;

        [Display(Name = "Đã gửi mail")]
        public bool DaGuiEmail { get; set; } = false;

        [Display(Name = "Ghi chú")]
        [StringLength(200)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaThuoc")]
        public virtual Thuoc? Thuoc { get; set; }
    }
}
