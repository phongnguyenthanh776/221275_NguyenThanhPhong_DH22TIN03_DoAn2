using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("NguyCoBenhLy", Schema = "dbo")]
public class NguyCoBenhLy
{
    [Key]
    public int MaNguyCo { get; set; }
    public int MaNguoiDung { get; set; }
    public string TenBenh { get; set; }
    public double? XacSuat { get; set; }
    public DateTime NgayDanhGia { get; set; }
}
