using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SucKhoeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;

    public SucKhoeController(AppDbContext context)
    {
        _context = context;
        _aiService = new AIService();
    }

    [HttpPost("chiso")]
    public async Task<IActionResult> ThemChiSo(ChiSoSucKhoe chiSo)
    {
        chiSo.NgayDo = DateTime.Now;
        _context.ChiSoSucKhoe.Add(chiSo);
        await _context.SaveChangesAsync();

        // Tính BMI
        var thongTin = await _context.ThongTinCaNhan
            .FirstOrDefaultAsync(t => t.MaNguoiDung == chiSo.MaNguoiDung);
        
        if (thongTin != null && thongTin.ChieuCao.HasValue && chiSo.CanNang.HasValue)
        {
            double bmi = chiSo.CanNang.Value / Math.Pow(thongTin.ChieuCao.Value / 100, 2);
            var trangThai = _aiService.PhanTichBMI(bmi);
            
            _context.LichSuBMI.Add(new LichSuBMI
            {
                MaNguoiDung = chiSo.MaNguoiDung,
                NgayTinh = DateTime.Now,
                GiaTriBMI = bmi,
                TrangThai = trangThai
            });

            // Phân tích nguy cơ
            if (chiSo.DuongHuyet.HasValue)
            {
                double nguyCoTD = _aiService.TinhNguyCoTieuDuong(bmi, chiSo.DuongHuyet.Value, thongTin.Tuoi ?? 0);
                if (nguyCoTD > 0.5)
                {
                    _context.NguyCoBenhLy.Add(new NguyCoBenhLy
                    {
                        MaNguoiDung = chiSo.MaNguoiDung,
                        TenBenh = "Tiểu đường",
                        XacSuat = nguyCoTD,
                        NgayDanhGia = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Đã lưu chỉ số sức khỏe" });
    }

    [HttpGet("lichsu/{maNguoiDung}")]
    public async Task<IActionResult> LayLichSu(int maNguoiDung)
    {
        var lichSu = await _context.ChiSoSucKhoe
            .Where(c => c.MaNguoiDung == maNguoiDung)
            .OrderByDescending(c => c.NgayDo)
            .Take(30)
            .ToListAsync();
        
        return Ok(lichSu);
    }
}
