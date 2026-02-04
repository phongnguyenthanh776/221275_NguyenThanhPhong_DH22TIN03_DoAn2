using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;
    private readonly GoogleAIService _googleAIService;
    private readonly ILogger<AIController> _logger;

    public AIController(AppDbContext context, ILogger<AIController> logger, GoogleAIService googleAIService)
    {
        _context = context;
        _aiService = new AIService();
        _googleAIService = googleAIService;
        _logger = logger;
    }

    [HttpPost("phantich/{maNguoiDung}")]
    public async Task<IActionResult> PhanTichSucKhoe(int maNguoiDung, [FromQuery] bool useGemini = false)
    {
        try
        {
            var thongTin = await _context.ThongTinCaNhan
                .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);

            var chiSo = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .FirstOrDefaultAsync();

            var thoiQuen = await _context.ThoiQuenSinhHoat
                .Where(t => t.MaNguoiDung == maNguoiDung)
                .OrderByDescending(t => t.MaThoiQuen)
                .FirstOrDefaultAsync();

            if (thongTin == null || chiSo == null)
                return BadRequest(new { message = "Chưa đủ dữ liệu để phân tích" });

            double bmi = chiSo.CanNang.Value / Math.Pow(thongTin.ChieuCao.Value / 100, 2);

            string baoCaoAI;
            string aiNguon;

            if (useGemini && _googleAIService.IsConfigured())
            {
                string thongTinInput = $@"
Thông tin người dùng:
- Họ tên: {thongTin.HoTen}
- Tuổi: {thongTin.Tuoi}
- Giới tính: {thongTin.GioiTinh}
- Chiều cao: {thongTin.ChieuCao} cm
- Cân nặng: {chiSo.CanNang} kg
- BMI: {bmi:F1}
- Huyết áp: {chiSo.HuyetAp}
- Nhịp tim: {chiSo.NhipTim} bpm
- Đường huyết: {chiSo.DuongHuyet} mg/dL
- Giờ ngủ: {thoiQuen?.SoGioNgu} giờ/ngày
- Tập luyện: {thoiQuen?.ThoiGianTapLuyen} phút/ngày
- Hút thuốc: {(thoiQuen?.HutThuoc ?? false ? "Có" : "Không")}
";

                var geminiResult = await _googleAIService.PhanTichSucKhoeAsync(thongTinInput);
                
                // Chỉ fallback nếu response bắt đầu bằng [
                if (geminiResult.StartsWith("["))
                {
                    _logger.LogWarning($"Gemini error code: {geminiResult}. Falling back to rule-based.");
                    baoCaoAI = _aiService.TaoBaoCaoAI(
                        bmi,
                        chiSo.HuyetAp ?? "120/80",
                        chiSo.DuongHuyet,
                        chiSo.NhipTim,
                        thongTin.Tuoi ?? 30,
                        thoiQuen?.HutThuoc ?? false,
                        thoiQuen?.UongRuouBia ?? false
                    );
                    aiNguon = "RuleBased (Gemini lỗi)";
                }
                else
                {
                    // Gemini thành công - response là text bình thường
                    baoCaoAI = geminiResult;
                    aiNguon = "Gemini";
                    _logger.LogInformation($"Gemini analysis successful");
                }
            }
            else
            {
                // Không dùng Gemini (useGemini=false hoặc chưa config)
                baoCaoAI = _aiService.TaoBaoCaoAI(
                    bmi,
                    chiSo.HuyetAp ?? "120/80",
                    chiSo.DuongHuyet,
                    chiSo.NhipTim,
                    thongTin.Tuoi ?? 30,
                    thoiQuen?.HutThuoc ?? false,
                    thoiQuen?.UongRuouBia ?? false
                );
                aiNguon = "RuleBased";
            }

            var goiYAI = new GoiYSucKhoeAI
            {
                MaNguoiDung = maNguoiDung,
                NoiDungGoiY = baoCaoAI,
                NgayTao = DateTime.Now
            };
            _context.GoiYSucKhoeAI.Add(goiYAI);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                bmi = Math.Round(bmi, 2),
                trangThaiBMI = _aiService.PhanTichBMI(bmi),
                baoCaoAI = baoCaoAI,
                aiNguon = aiNguon
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi phân tích AI: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    [HttpPost("goiy-thucdon/{maNguoiDung}")]
    public async Task<IActionResult> GoiYThucDonAI(int maNguoiDung)
    {
        try
        {
            var thongTin = await _context.ThongTinCaNhan
                .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);

            var chiSo = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .FirstOrDefaultAsync();

            if (thongTin == null || chiSo == null)
                return BadRequest(new { message = "Chưa đủ dữ liệu" });

            double bmi = chiSo.CanNang.Value / Math.Pow(thongTin.ChieuCao.Value / 100, 2);

            string thongTinInput = $@"
Thông tin:
- Tuổi: {thongTin.Tuoi}
- Giới tính: {thongTin.GioiTinh}
- BMI: {bmi:F1}
- Tình trạng: {_aiService.PhanTichBMI(bmi)}
- Đường huyết: {chiSo.DuongHuyet} mg/dL
";

            string goiY = await _googleAIService.GoiYThucDonAsync(thongTinInput);

            return Ok(new { goiY });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi gợi ý thực đơn: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
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

    [HttpGet("status")]
    public IActionResult GetAIStatus()
    {
        return Ok(new
        {
            geminiConfigured = _googleAIService.IsConfigured(),
            model = _googleAIService.GetModel()
        });
    }

    [HttpGet("chat-history/{maNguoiDung}")]
    public async Task<IActionResult> GetChatHistory(int maNguoiDung)
    {
        return Ok(new List<object>());
    }
}
