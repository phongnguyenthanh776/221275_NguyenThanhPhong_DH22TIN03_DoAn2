using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthManagement.Models
{
    /// <summary>
    /// Bảng lịch sử chat với AI Chatbot
    /// CHÚ Ý: Hiện tại chưa triển khai, đây là placeholder cho tương lai
    /// </summary>
    public class LichSuChatBot
    {
        [Key]
        public int MaLichSuChat { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(2000)]
        public string CauHoi { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        public string CauTraLoi { get; set; } = string.Empty;

        [Required]
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        public bool HuuIch { get; set; } = true; // User có thấy hữu ích không

        // Navigation Property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
