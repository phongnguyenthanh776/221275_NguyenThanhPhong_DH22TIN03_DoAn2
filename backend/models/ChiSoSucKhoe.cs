using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ChiSoSucKhoe", Schema = "dbo")]
public class ChiSoSucKhoe
{
    [Key]
    public int MaChiSo { get; set; }
    public int MaNguoiDung { get; set; }
    public DateTime NgayDo { get; set; }
    public double? CanNang { get; set; }
    public string HuyetAp { get; set; }
    public int? NhipTim { get; set; }
    public double? DuongHuyet { get; set; }
}
