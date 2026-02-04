using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ThongTinCaNhan", Schema = "dbo")]
public class ThongTinCaNhan
{
    [Key]
    public int MaThongTin { get; set; }
    public int MaNguoiDung { get; set; }
    public string HoTen { get; set; }
    public int? Tuoi { get; set; }
    public string GioiTinh { get; set; }
    public double? ChieuCao { get; set; }
    public double? CanNang { get; set; }
}
