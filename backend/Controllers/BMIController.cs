using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BMIController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;

    public BMIController(AppDbContext context)
    {
        _context = context;
        _aiService = new AIService();
    }

    [HttpGet("lichsu/{maNguoiDung}")]
    public async Task<IActionResult> LayLichSuBMI(int maNguoiDung)
    {
        var lichSu = await _context.LichSuBMI
            .Where(b => b.MaNguoiDung == maNguoiDung)
            .OrderByDescending(b => b.NgayTinh)
            .Take(30)
            .ToListAsync();
        
        return Ok(lichSu);
    }

    [HttpPost("tinh")]
    public async Task<IActionResult> TinhBMI([FromBody] BMIRequest request)
    {
        double bmi = request.CanNang / Math.Pow(request.ChieuCao / 100, 2);
        string trangThai = _aiService.PhanTichBMI(bmi);

        var lichSuBMI = new LichSuBMI
        {
            MaNguoiDung = request.MaNguoiDung,
            NgayTinh = DateTime.Now,
            GiaTriBMI = Math.Round(bmi, 2),
            TrangThai = trangThai
        };

        _context.LichSuBMI.Add(lichSuBMI);
        await _context.SaveChangesAsync();

        // Tạo cảnh báo nếu cần
        if (trangThai == "Béo phì" || trangThai == "Gầy")
        {
            var canhBao = new CanhBaoSucKhoe
            {
                MaNguoiDung = request.MaNguoiDung,
                NoiDung = $"BMI của bạn đang ở mức {trangThai}. Hãy chú ý sức khỏe!",
                NgayCanhBao = DateTime.Now,
                TrangThai = "Chưa xem"
            };
            _context.CanhBaoSucKhoe.Add(canhBao);
            await _context.SaveChangesAsync();
        }

        return Ok(new { bmi = Math.Round(bmi, 2), trangThai });
    }
}

public class BMIRequest
{
    public int MaNguoiDung { get; set; }
    public double CanNang { get; set; }
    public double ChieuCao { get; set; }
}
