using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MonAnController : ControllerBase
{
    private readonly AppDbContext _context;

    public MonAnController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> LayDanhSachMonAn()
    {
        var danhSach = await _context.MonAn.ToListAsync();
        return Ok(danhSach);
    }

    [HttpPost]
    public async Task<IActionResult> ThemMonAn(MonAn monAn)
    {
        _context.MonAn.Add(monAn);
        await _context.SaveChangesAsync();
        return Ok(monAn);
    }

    [HttpGet("goiy/{maNguoiDung}")]
    public async Task<IActionResult> GoiYMonAn(int maNguoiDung)
    {
        var aiService = new AIService();
        
        // Lấy BMI mới nhất
        var bmiMoiNhat = await _context.LichSuBMI
            .Where(b => b.MaNguoiDung == maNguoiDung)
            .OrderByDescending(b => b.NgayTinh)
            .FirstOrDefaultAsync();

        if (bmiMoiNhat == null)
            return Ok(new List<string> { "Chưa có dữ liệu BMI" });

        // Kiểm tra nguy cơ tiểu đường
        var nguyCo = await _context.NguyCoBenhLy
            .Where(n => n.MaNguoiDung == maNguoiDung && n.TenBenh == "Tiểu đường")
            .OrderByDescending(n => n.NgayDanhGia)
            .FirstOrDefaultAsync();

        bool coTieuDuong = nguyCo != null && nguyCo.XacSuat > 0.5;

        var goiY = aiService.GoiYThucDon(bmiMoiNhat.TrangThai, coTieuDuong);
        
        return Ok(goiY);
    }
}
