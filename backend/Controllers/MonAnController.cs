using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class MonAnController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AIService _aiService;
    private readonly ILogger<MonAnController> _logger;

    public MonAnController(AppDbContext context, ILogger<MonAnController> logger)
    {
        _context = context;
        _aiService = new AIService();
        _logger = logger;
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
        try
        {
            _logger.LogInformation($"Tạo gợi ý thực đơn cho user {maNguoiDung}");

            // Lấy thông tin cá nhân
            var thongTin = await _context.ThongTinCaNhan
                .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);

            // Lấy BMI mới nhất
            var bmiMoiNhat = await _context.LichSuBMI
                .Where(b => b.MaNguoiDung == maNguoiDung)
                .OrderByDescending(b => b.NgayTinh)
                .FirstOrDefaultAsync();

            // Lấy chỉ số sức khỏe mới nhất
            var chiSo = await _context.ChiSoSucKhoe
                .Where(c => c.MaNguoiDung == maNguoiDung)
                .OrderByDescending(c => c.NgayDo)
                .FirstOrDefaultAsync();

            // Lấy nguy cơ bệnh lý
            var nguyCoTieuDuong = await _context.NguyCoBenhLy
                .Where(n => n.MaNguoiDung == maNguoiDung && n.TenBenh == "Tiểu đường")
                .OrderByDescending(n => n.NgayDanhGia)
                .FirstOrDefaultAsync();

            var nguyCoCaoHuyetAp = await _context.NguyCoBenhLy
                .Where(n => n.MaNguoiDung == maNguoiDung && n.TenBenh == "Cao huyết áp")
                .OrderByDescending(n => n.NgayDanhGia)
                .FirstOrDefaultAsync();

            // Lấy thói quen
            var thoiQuen = await _context.ThoiQuenSinhHoat
                .Where(t => t.MaNguoiDung == maNguoiDung)
                .OrderByDescending(t => t.MaThoiQuen)
                .FirstOrDefaultAsync();

            if (bmiMoiNhat == null || thongTin == null)
            {
                return Ok(new { 
                    message = "Chưa có dữ liệu BMI. Vui lòng cập nhật thông tin cá nhân và chỉ số sức khỏe.",
                    goiYChung = new List<string> {
                        "🥗 Ăn nhiều rau xanh",
                        "🍎 Ăn trái cây hàng ngày",
                        "💧 Uống đủ 2 lít nước/ngày",
                        "🍚 Ưu tiên ngũ cốc nguyên hạt"
                    }
                });
            }

            bool coTieuDuong = nguyCoTieuDuong != null && nguyCoTieuDuong.XacSuat > 50;
            bool coCaoHuyetAp = nguyCoCaoHuyetAp != null && nguyCoCaoHuyetAp.XacSuat > 50;

            // Gợi ý từ AI
            var goiYChung = _aiService.GoiYThucDon(bmiMoiNhat.TrangThai, coTieuDuong, coCaoHuyetAp);

            // Lấy món ăn phù hợp từ database
            var monAnPhuHop = await LayMonAnPhuHop(bmiMoiNhat.TrangThai, coTieuDuong, coCaoHuyetAp);

            // Tạo thực đơn theo bữa
            var thucDonTheoNgay = TaoThucDonTheoNgay(monAnPhuHop, bmiMoiNhat.TrangThai);

            // Tính tổng dinh dưỡng
            var tongDinhDuong = TinhTongDinhDuong(thucDonTheoNgay);

            return Ok(new
            {
                trangThaiBMI = bmiMoiNhat.TrangThai,
                bmi = bmiMoiNhat.GiaTriBMI,
                coNguyCoTieuDuong = coTieuDuong,
                coNguyCoCaoHuyetAp = coCaoHuyetAp,
                goiYChung = goiYChung,
                thucDonTheoNgay = thucDonTheoNgay,
                tongDinhDuong = tongDinhDuong,
                cacMonAnKhac = monAnPhuHop.Select(m => new {
                    m.MaMonAn,
                    m.TenMonAn,
                    m.Calo,
                    m.ChatDam,
                    m.ChatBeo,
                    m.ChatBot
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi gợi ý thực đơn: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    private async Task<List<MonAn>> LayMonAnPhuHop(string trangThaiBMI, bool coTieuDuong, bool coCaoHuyetAp)
    {
        var query = _context.MonAn.AsQueryable();

        if (trangThaiBMI.Contains("Gầy"))
        {
            // Ưu tiên món giàu calo và protein
            query = query.Where(m => m.Calo > 150 && m.ChatDam > 15);
        }
        else if (trangThaiBMI.Contains("Thừa cân") || trangThaiBMI.Contains("Béo phì"))
        {
            // Ưu tiên món ít calo, ít béo
            query = query.Where(m => m.Calo < 200 && m.ChatBeo < 10);
        }

        if (coTieuDuong)
        {
            // Ưu tiên món ít carbs
            query = query.Where(m => m.ChatBot < 20);
        }

        var result = await query.ToListAsync();
        
        // Nếu không tìm thấy món phù hợp, lấy tất cả
        if (result.Count == 0)
            result = await _context.MonAn.ToListAsync();

        return result;
    }

    private Dictionary<string, List<object>> TaoThucDonTheoNgay(List<MonAn> danhSachMon, string trangThaiBMI)
    {
        var thucDon = new Dictionary<string, List<object>>();
        var random = new Random();

        // Chia món ăn theo nhóm
        var monChinh = danhSachMon.Where(m => m.Calo > 150).ToList();
        var monPhu = danhSachMon.Where(m => m.Calo <= 150).ToList();

        // Sáng
        thucDon["Bữa sáng (7:00 - 8:00)"] = new List<object>();
        if (monPhu.Any())
        {
            var monSang = monPhu.OrderBy(x => random.Next()).Take(2).ToList();
            foreach (var mon in monSang)
            {
                thucDon["Bữa sáng (7:00 - 8:00)"].Add(new {
                    mon.TenMonAn,
                    mon.Calo,
                    KhauPhan = "1 phần"
                });
            }
        }

        // Trưa
        thucDon["Bữa trưa (12:00 - 13:00)"] = new List<object>();
        if (monChinh.Any())
        {
            var monTrua = monChinh.OrderBy(x => random.Next()).Take(3).ToList();
            foreach (var mon in monTrua)
            {
                thucDon["Bữa trưa (12:00 - 13:00)"].Add(new {
                    mon.TenMonAn,
                    mon.Calo,
                    KhauPhan = "1 phần"
                });
            }
        }

        // Xế chiều (snack)
        thucDon["Bữa phụ (15:00 - 16:00)"] = new List<object>();
        if (monPhu.Any())
        {
            var monXe = monPhu.OrderBy(x => random.Next()).Take(1).ToList();
            foreach (var mon in monXe)
            {
                thucDon["Bữa phụ (15:00 - 16:00)"].Add(new {
                    mon.TenMonAn,
                    mon.Calo,
                    KhauPhan = "1 phần nhỏ"
                });
            }
        }

        // Tối
        thucDon["Bữa tối (18:00 - 19:00)"] = new List<object>();
        if (monChinh.Any())
        {
            var monToi = monChinh.OrderBy(x => random.Next()).Take(2).ToList();
            foreach (var mon in monToi)
            {
                thucDon["Bữa tối (18:00 - 19:00)"].Add(new {
                    mon.TenMonAn,
                    mon.Calo,
                    KhauPhan = trangThaiBMI.Contains("Thừa cân") || trangThaiBMI.Contains("Béo phì") 
                        ? "1/2 phần" 
                        : "1 phần"
                });
            }
        }

        return thucDon;
    }

    private object TinhTongDinhDuong(Dictionary<string, List<object>> thucDon)
    {
        double tongCalo = 0;
        int soBua = 0;

        foreach (var bua in thucDon.Values)
        {
            foreach (dynamic mon in bua)
            {
                tongCalo += mon.Calo;
                soBua++;
            }
        }

        return new
        {
            TongCalo = Math.Round(tongCalo, 0),
            SoBuaAn = soBua,
            CaloTrungBinhMoiBua = soBua > 0 ? Math.Round(tongCalo / soBua, 0) : 0,
            DanhGia = tongCalo < 1500 ? "Ít calo - Phù hợp giảm cân" :
                      tongCalo < 2000 ? "Vừa phải - Cân đối" :
                      tongCalo < 2500 ? "Cao - Phù hợp tăng cân" :
                      "Rất cao - Cần điều chỉnh"
        };
    }

    [HttpPost("luu-thucdon")]
    public async Task<IActionResult> LuuThucDon([FromBody] ThucDonRequest request)
    {
        try
        {
            foreach (var maMonAn in request.DanhSachMonAn)
            {
                var thucDon = new ThucDon
                {
                    MaNguoiDung = request.MaNguoiDung,
                    Ngay = DateTime.Now.Date,
                    MaMonAn = maMonAn
                };
                _context.ThucDon.Add(thucDon);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã lưu thực đơn!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }

    [HttpGet("thucdon-da-luu/{maNguoiDung}")]
    public async Task<IActionResult> LayThucDonDaLuu(int maNguoiDung, [FromQuery] DateTime? ngay)
    {
        var ngayXem = ngay ?? DateTime.Now.Date;

        var thucDon = await _context.ThucDon
            .Where(t => t.MaNguoiDung == maNguoiDung && t.Ngay == ngayXem)
            .Join(_context.MonAn,
                  td => td.MaMonAn,
                  ma => ma.MaMonAn,
                  (td, ma) => new {
                      td.MaThucDon,
                      td.Ngay,
                      ma.TenMonAn,
                      ma.Calo,
                      ma.ChatDam,
                      ma.ChatBeo,
                      ma.ChatBot
                  })
            .ToListAsync();

        return Ok(thucDon);
    }
}

public class ThucDonRequest
{
    public int MaNguoiDung { get; set; }
    public List<int> DanhSachMonAn { get; set; }
}
