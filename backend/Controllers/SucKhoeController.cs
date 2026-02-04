using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class SucKhoeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;
    private readonly ILogger<SucKhoeController> _logger;

    public SucKhoeController(AppDbContext context, ILogger<SucKhoeController> logger)
    {
        _context = context;
        _aiService = new AIService();
        _logger = logger;
    }

    [HttpPost("chiso")]
    public async Task<IActionResult> ThemChiSo([FromBody] ChiSoSucKhoe chiSo)
    {
        try
        {
            chiSo.NgayDo = DateTime.Now;
            _context.ChiSoSucKhoe.Add(chiSo);
            await _context.SaveChangesAsync();

            // Tính BMI nếu có thông tin
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
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi thêm chỉ số: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    [HttpGet("lichsu/{maNguoiDung}")]
    public async Task<IActionResult> LayLichSu(int maNguoiDung)
    {
        try
        {
            var lichSu = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .Take(30)
                .ToListAsync();
            
            return Ok(lichSu);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy lịch sử: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    [HttpGet("chiso-gan-nhat/{maNguoiDung}")]
    public async Task<IActionResult> LayChiSoGanNhat(int maNguoiDung)
    {
        try
        {
            var thongTin = await _context.ThongTinCaNhan
                .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);

            var chiSo = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .FirstOrDefaultAsync();

            var bmi = await _context.LichSuBMI
                .Where(b => b.MaNguoiDung == maNguoiDung)
                .OrderByDescending(b => b.NgayTinh)
                .FirstOrDefaultAsync();

            var thoiQuen = await _context.ThoiQuenSinhHoat
                .Where(t => t.MaNguoiDung == maNguoiDung)
                .OrderByDescending(t => t.MaThoiQuen)
                .FirstOrDefaultAsync();

            if (thongTin == null)
            {
                return NotFound(new { message = "Chưa có thông tin cá nhân" });
            }

            return Ok(new
            {
                thongTinCaNhan = thongTin,
                chiSoSucKhoe = chiSo,
                bmi = bmi,
                thoiQuen = thoiQuen,
                daySoGanNhat = chiSo?.NgayDo.ToString("dd/MM/yyyy") ?? "Chưa có"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy chỉ số gần nhất: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    [HttpGet("tong-quan/{maNguoiDung}")]
    public async Task<IActionResult> LayTongQuan(int maNguoiDung)
    {
        try
        {
            _logger.LogInformation($"API tong-quan called for user: {maNguoiDung}");

            // Lấy chỉ số gần nhất
            var chiSoGanNhat = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .FirstOrDefaultAsync();

            // Lấy BMI gần nhất
            var bmiGanNhat = await _context.LichSuBMI
                .Where(b => b.MaNguoiDung == maNguoiDung)
                .OrderByDescending(b => b.NgayTinh)
                .FirstOrDefaultAsync();

            // Lấy lịch sử 7 ngày
            var lichSu7Ngay = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .Take(7)
                .ToListAsync();

            // Tính xu hướng
            string xuHuongCanNang = "Chưa đủ dữ liệu";
            if (lichSu7Ngay.Count >= 2)
            {
                var first = lichSu7Ngay.Last().CanNang ?? 0;
                var last = lichSu7Ngay.First().CanNang ?? 0;
                if (last > first + 1) xuHuongCanNang = "Tăng ⬆️";
                else if (last < first - 1) xuHuongCanNang = "Giảm ⬇️";
                else xuHuongCanNang = "Ổn định ➡️";
            }

            // Đếm cảnh báo
            var soCanhBaoChuaXem = await _context.CanhBaoSucKhoe
                .CountAsync(c => c.MaNguoiDung == maNguoiDung && c.TrangThai == "Chưa xem");

            var result = new
            {
                chiSoGanNhat = chiSoGanNhat,
                bmiGanNhat = bmiGanNhat,
                lichSu7Ngay = lichSu7Ngay.Select(l => new {
                    l.NgayDo,
                    l.CanNang,
                    l.HuyetAp,
                    l.NhipTim,
                    l.DuongHuyet
                }).ToList(),
                xuHuongCanNang = xuHuongCanNang,
                soCanhBaoChuaXem = soCanhBaoChuaXem,
                ngayCapNhatCuoi = chiSoGanNhat?.NgayDo.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có dữ liệu"
            };

            _logger.LogInformation($"Returning data: {result}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in tong-quan: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    private string TinhXuHuong(List<double> values)
    {
        if (values.Count < 2) return "Chưa đủ dữ liệu";

        var first = values.Last();
        var last = values.First();

        if (last > first + 1) return "Tăng ⬆️";
        if (last < first - 1) return "Giảm ⬇️";
        return "Ổn định ➡️";
    }
}
