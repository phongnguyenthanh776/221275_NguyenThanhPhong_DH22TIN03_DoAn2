using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("LichSuBMI", Schema = "dbo")]
public class LichSuBMI
{
    [Key]
    public int MaBMI { get; set; }
    public int MaNguoiDung { get; set; }
    public DateTime NgayTinh { get; set; }
    public double GiaTriBMI { get; set; }
    public string TrangThai { get; set; }
}
