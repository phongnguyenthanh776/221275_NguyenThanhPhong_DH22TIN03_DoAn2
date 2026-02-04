using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class NguoiDungController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<NguoiDungController> _logger;

    public NguoiDungController(AppDbContext context, ILogger<NguoiDungController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NguoiDung nguoiDung)
    {
        try
        {
            _logger.LogInformation($"Đăng ký tài khoản: {nguoiDung.TenDangNhap}");

            // Kiểm tra tên đăng nhập đã tồn tại
            var existingUser = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == nguoiDung.TenDangNhap.ToLower());

            if (existingUser != null)
            {
                _logger.LogWarning($"Tên đăng nhập đã tồn tại: {nguoiDung.TenDangNhap}");
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });
            }

            // Kiểm tra email đã tồn tại
            if (!string.IsNullOrEmpty(nguoiDung.Email))
            {
                var existingEmail = await _context.NguoiDung
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == nguoiDung.Email.ToLower());

                if (existingEmail != null)
                {
                    _logger.LogWarning($"Email đã được sử dụng: {nguoiDung.Email}");
                    return BadRequest(new { message = "Email đã được sử dụng!" });
                }
            }

            // Thêm người dùng mới
            _context.NguoiDung.Add(nguoiDung);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Đăng ký thành công: {nguoiDung.TenDangNhap} (ID: {nguoiDung.MaNguoiDung})");

            return Ok(new { 
                message = "Đăng ký thành công",
                maNguoiDung = nguoiDung.MaNguoiDung,
                tenDangNhap = nguoiDung.TenDangNhap
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi đăng ký: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation($"Đăng nhập: {request.TenDangNhap}");

            var user = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == request.TenDangNhap.ToLower() 
                                       && u.MatKhau == request.MatKhau);
            
            if (user == null)
            {
                _logger.LogWarning($"Đăng nhập thất bại: {request.TenDangNhap}");
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            _logger.LogInformation($"Đăng nhập thành công: {request.TenDangNhap}");
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi đăng nhập: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.NguoiDung.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.NguoiDung.FindAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }

    [HttpDelete("duplicate/{username}")]
    public async Task<IActionResult> DeleteDuplicate(string username)
    {
        try
        {
            // Lấy tất cả user trùng tên
            var duplicates = await _context.NguoiDung
                .Where(u => u.TenDangNhap.ToLower() == username.ToLower())
                .OrderBy(u => u.MaNguoiDung)
                .ToListAsync();

            if (duplicates.Count <= 1)
            {
                return Ok(new { message = "Không có bản ghi trùng lặp" });
            }

            // Giữ lại bản ghi đầu tiên, xóa các bản còn lại
            var toDelete = duplicates.Skip(1).ToList();
            _context.NguoiDung.RemoveRange(toDelete);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Đã xóa {toDelete.Count} bản ghi trùng lặp",
                deleted = toDelete.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi: " + ex.Message });
        }
    }
}

public class LoginRequest
{
    public string TenDangNhap { get; set; }
    public string MatKhau { get; set; }
}
