using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Nhật ký uống nước hằng ngày
    /// </summary>
    public class UongNuoc
    {
        [Key]
        public int MaUongNuoc { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [Display(Name = "Số ml")]
        [Range(50, 5000)]
        public int SoMl { get; set; }

        [Required]
        [Display(Name = "Thời gian")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        [StringLength(300)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
