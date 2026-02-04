using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ThucDon", Schema = "dbo")]
public class ThucDon
{
    [Key]
    public int MaThucDon { get; set; }
    public int MaNguoiDung { get; set; }
    public DateTime Ngay { get; set; }
    public int MaMonAn { get; set; }
}
