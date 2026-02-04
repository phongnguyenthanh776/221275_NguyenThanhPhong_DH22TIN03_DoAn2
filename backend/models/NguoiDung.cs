using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("NguoiDung", Schema = "dbo")]
public class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TenDangNhap { get; set; }
    
    [Required]
    [StringLength(255)]
    public string MatKhau { get; set; }
    
    [StringLength(100)]
    public string Email { get; set; }
    
    [StringLength(50)]
    public string VaiTro { get; set; }
}
