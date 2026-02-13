using System.ComponentModel.DataAnnotations;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng vai trò: Admin, NguoiDung
    /// </summary>
    public class VaiTro
    {
        [Key]
        public int MaVaiTro { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVaiTro { get; set; } = string.Empty; // Admin, NguoiDung

        [StringLength(200)]
        public string? MoTa { get; set; }

        // Navigation Properties
        public virtual ICollection<NguoiDung> NguoiDung { get; set; } = new List<NguoiDung>();
    }
}
