using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ThoiQuenSinhHoat", Schema = "dbo")]
public class ThoiQuenSinhHoat
{
    [Key]
    public int MaThoiQuen { get; set; }
    public int MaNguoiDung { get; set; }
    public double? SoGioNgu { get; set; }
    public int? ThoiGianTapLuyen { get; set; }
    public bool? HutThuoc { get; set; }
    public bool? UongRuouBia { get; set; }
}
