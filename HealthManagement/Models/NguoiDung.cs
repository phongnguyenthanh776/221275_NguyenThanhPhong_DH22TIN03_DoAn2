using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng thông tin người dùng (extends IdentityUser)
    /// </summary>
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        public DateTime NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; } = "Nam"; // Nam, Nữ, Khác

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [Required]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Foreign Key
        public int MaVaiTro { get; set; }

        public string? IdentityUserId { get; set; } // Link to ASP.NET Identity

        // Navigation Properties
        [ForeignKey("MaVaiTro")]
        public virtual VaiTro? VaiTro { get; set; }

        public virtual HoSoSucKhoe? HoSoSucKhoe { get; set; }
        public virtual ICollection<ChiSoSucKhoe> ChiSoSucKhoe { get; set; } = new List<ChiSoSucKhoe>();
        public virtual ICollection<LichSuBMI> LichSuBMI { get; set; } = new List<LichSuBMI>();
        public virtual ICollection<DuDoanAI> DuDoanAI { get; set; } = new List<DuDoanAI>();
        public virtual ICollection<GiacNgu> GiacNgu { get; set; } = new List<GiacNgu>();
        public virtual ICollection<UongNuoc> UongNuoc { get; set; } = new List<UongNuoc>();
        public virtual ICollection<Thuoc> Thuoc { get; set; } = new List<Thuoc>();
        public virtual ICollection<WaterReminder> WaterReminders { get; set; } = new List<WaterReminder>();
    }
}
