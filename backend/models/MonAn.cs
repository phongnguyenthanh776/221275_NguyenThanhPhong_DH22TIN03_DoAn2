using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MonAn", Schema = "dbo")]
public class MonAn
{
    [Key]
    public int MaMonAn { get; set; }
    public string TenMonAn { get; set; }
    public double? Calo { get; set; }
    public double? ChatDam { get; set; }
    public double? ChatBeo { get; set; }
    public double? ChatBot { get; set; }
}
