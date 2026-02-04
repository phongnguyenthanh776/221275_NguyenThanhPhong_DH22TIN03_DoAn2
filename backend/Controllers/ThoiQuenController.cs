using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ThoiQuenController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThoiQuenController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{maNguoiDung}")]
    public async Task<IActionResult> LayThoiQuen(int maNguoiDung)
    {
        var thoiQuen = await _context.ThoiQuenSinhHoat
            .Where(t => t.MaNguoiDung == maNguoiDung)
            .OrderByDescending(t => t.MaThoiQuen)
            .FirstOrDefaultAsync();
        
        if (thoiQuen == null)
            return NotFound();
        
        return Ok(thoiQuen);
    }

    [HttpPost]
    public async Task<IActionResult> LuuThoiQuen(ThoiQuenSinhHoat thoiQuen)
    {
        _context.ThoiQuenSinhHoat.Add(thoiQuen);
        await _context.SaveChangesAsync();
        return Ok(thoiQuen);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> CapNhatThoiQuen(int id, ThoiQuenSinhHoat thoiQuen)
    {
        if (id != thoiQuen.MaThoiQuen)
            return BadRequest();

        _context.Entry(thoiQuen).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(thoiQuen);
    }
}
