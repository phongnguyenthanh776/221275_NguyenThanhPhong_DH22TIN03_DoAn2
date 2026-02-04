using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("GoiYSucKhoeAI", Schema = "dbo")]
public class GoiYSucKhoeAI
{
    [Key]
    public int MaGoiY { get; set; }
    public int MaNguoiDung { get; set; }
    public string NoiDungGoiY { get; set; }
    public DateTime NgayTao { get; set; }
}
