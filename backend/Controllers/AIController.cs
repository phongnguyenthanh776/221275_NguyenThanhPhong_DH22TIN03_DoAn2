using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;

    public AIController(AppDbContext context)
    {
        _context = context;
        _aiService = new AIService();
    }

    [HttpPost("phantich/{maNguoiDung}")]
    public async Task<IActionResult> PhanTichSucKhoe(int maNguoiDung)
    {
        // Lấy thông tin cá nhân
        var thongTin = await _context.ThongTinCaNhan
            .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);

        // Lấy chỉ số sức khỏe mới nhất
        var chiSo = await _context.ChiSoSucKhoe
            .Where(c => c.MaNguoiDung == maNguoiDung)
            .OrderByDescending(c => c.NgayDo)
            .FirstOrDefaultAsync();

        if (thongTin == null || chiSo == null)
            return BadRequest("Chưa đủ dữ liệu để phân tích");

        // Tính BMI
        double bmi = chiSo.CanNang.Value / Math.Pow(thongTin.ChieuCao.Value / 100, 2);

        // Phân tích nguy cơ
        double nguyCoTieuDuong = 0;
        double nguyCoCaoHuyetAp = 0;

        if (chiSo.DuongHuyet.HasValue)
        {
            nguyCoTieuDuong = _aiService.TinhNguyCoTieuDuong(bmi, chiSo.DuongHuyet.Value, thongTin.Tuoi ?? 0);
            
            if (nguyCoTieuDuong > 0.5)
            {
                _context.NguyCoBenhLy.Add(new NguyCoBenhLy
                {
                    MaNguoiDung = maNguoiDung,
                    TenBenh = "Tiểu đường",
                    XacSuat = nguyCoTieuDuong,
                    NgayDanhGia = DateTime.Now
                });
            }
        }

        if (!string.IsNullOrEmpty(chiSo.HuyetAp))
        {
            nguyCoCaoHuyetAp = _aiService.TinhNguyCoCaoHuyetAp(chiSo.HuyetAp, bmi, thongTin.Tuoi ?? 0);
            
            if (nguyCoCaoHuyetAp > 0.5)
            {
                _context.NguyCoBenhLy.Add(new NguyCoBenhLy
                {
                    MaNguoiDung = maNguoiDung,
                    TenBenh = "Cao huyết áp",
                    XacSuat = nguyCoCaoHuyetAp,
                    NgayDanhGia = DateTime.Now
                });
            }
        }

        // Tạo gợi ý AI
        string goiY = TaoGoiYAI(bmi, nguyCoTieuDuong, nguyCoCaoHuyetAp);
        
        _context.GoiYSucKhoeAI.Add(new GoiYSucKhoeAI
        {
            MaNguoiDung = maNguoiDung,
            NoiDungGoiY = goiY,
            NgayTao = DateTime.Now
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            bmi = Math.Round(bmi, 2),
            trangThaiBMI = _aiService.PhanTichBMI(bmi),
            nguyCoTieuDuong = Math.Round(nguyCoTieuDuong * 100, 2),
            nguyCoCaoHuyetAp = Math.Round(nguyCoCaoHuyetAp * 100, 2),
            goiY
        });
    }

    [HttpGet("goiy/{maNguoiDung}")]
    public async Task<IActionResult> LayGoiYAI(int maNguoiDung)
    {
        var goiY = await _context.GoiYSucKhoeAI
            .Where(g => g.MaNguoiDung == maNguoiDung)
            .OrderByDescending(g => g.NgayTao)
            .Take(10)
            .ToListAsync();
        
        return Ok(goiY);
    }

    private string TaoGoiYAI(double bmi, double nguyCoTieuDuong, double nguyCoCaoHuyetAp)
    {
        var goiY = new System.Text.StringBuilder();
        
        goiY.AppendLine("📊 PHÂN TÍCH SỨC KHỎE TỪ AI:");
        goiY.AppendLine();

        // Đánh giá BMI
        string trangThaiBMI = _aiService.PhanTichBMI(bmi);
        goiY.AppendLine($"✓ BMI: {Math.Round(bmi, 2)} - {trangThaiBMI}");
        
        if (trangThaiBMI == "Béo phì")
            goiY.AppendLine("  → Khuyến nghị: Giảm cân bằng chế độ ăn uống và tập luyện");
        else if (trangThaiBMI == "Gầy")
            goiY.AppendLine("  → Khuyến nghị: Tăng cường dinh dưỡng");
        
        goiY.AppendLine();

        // Đánh giá nguy cơ
        if (nguyCoTieuDuong > 0.5)
        {
            goiY.AppendLine($"⚠️ Nguy cơ tiểu đường: {Math.Round(nguyCoTieuDuong * 100)}%");
            goiY.AppendLine("  → Hạn chế đường, tinh bột");
            goiY.AppendLine("  → Tăng cường vận động");
            goiY.AppendLine();
        }

        if (nguyCoCaoHuyetAp > 0.5)
        {
            goiY.AppendLine($"⚠️ Nguy cơ cao huyết áp: {Math.Round(nguyCoCaoHuyetAp * 100)}%");
            goiY.AppendLine("  → Giảm muối trong ăn uống");
            goiY.AppendLine("  → Tránh stress");
            goiY.AppendLine();
        }

        // Lời khuyên chung
        goiY.AppendLine("💡 LỜI KHUYÊN:");
        goiY.AppendLine("  • Ngủ đủ 7-8 tiếng/ngày");
        goiY.AppendLine("  • Uống đủ 2 lít nước/ngày");
        goiY.AppendLine("  • Tập thể dục 30 phút/ngày");
        goiY.AppendLine("  • Ăn nhiều rau xanh");

        return goiY.ToString();
    }
}
