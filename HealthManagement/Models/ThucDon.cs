using System.ComponentModel.DataAnnotations;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng thực đơn (món ăn)
    /// </summary>
    public class ThucDon
    {
        [Key]
        public int MaMonAn { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên món ăn")]
        [StringLength(200)]
        public string TenMonAn { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? MoTa { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Calo { get; set; }

        [Range(0, 1000)]
        public decimal Protein { get; set; } // g

        [Range(0, 1000)]
        public decimal Carbs { get; set; } // g

        [Range(0, 1000)]
        public decimal Fat { get; set; } // g

        [StringLength(50)]
        public string? LoaiMonAn { get; set; } // Sáng, Trưa, Tối, Phụ

        [StringLength(500)]
        public string? HinhAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<ChiTietKeHoachAn> ChiTietKeHoachAn { get; set; } = new List<ChiTietKeHoachAn>();
    }
}
