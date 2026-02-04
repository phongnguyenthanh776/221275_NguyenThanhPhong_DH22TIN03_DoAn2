using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ThongTinCaNhanController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThongTinCaNhanController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{maNguoiDung}")]
    public async Task<IActionResult> LayThongTin(int maNguoiDung)
    {
        var thongTin = await _context.ThongTinCaNhan
            .FirstOrDefaultAsync(t => t.MaNguoiDung == maNguoiDung);
        
        if (thongTin == null)
            return NotFound();
        
        return Ok(thongTin);
    }

    [HttpPost]
    public async Task<IActionResult> TaoThongTin(ThongTinCaNhan thongTin)
    {
        _context.ThongTinCaNhan.Add(thongTin);
        await _context.SaveChangesAsync();
        return Ok(thongTin);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> CapNhatThongTin(int id, ThongTinCaNhan thongTin)
    {
        if (id != thongTin.MaThongTin)
            return BadRequest();

        _context.Entry(thongTin).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(thongTin);
    }
}
