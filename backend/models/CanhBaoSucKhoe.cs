using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CanhBaoSucKhoe", Schema = "dbo")]
public class CanhBaoSucKhoe
{
    [Key]
    public int MaCanhBao { get; set; }
    public int MaNguoiDung { get; set; }
    public string NoiDung { get; set; }
    public DateTime NgayCanhBao { get; set; }
    public string TrangThai { get; set; }
}
