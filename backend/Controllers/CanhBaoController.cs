using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CanhBaoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CanhBaoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{maNguoiDung}")]
    public async Task<IActionResult> LayCanhBao(int maNguoiDung)
    {
        var canhBao = await _context.CanhBaoSucKhoe
            .Where(c => c.MaNguoiDung == maNguoiDung)
            .OrderByDescending(c => c.NgayCanhBao)
            .Take(20)
            .ToListAsync();
        
        return Ok(canhBao);
    }

    [HttpPut("daxem/{id}")]
    public async Task<IActionResult> DanhDauDaXem(int id)
    {
        var canhBao = await _context.CanhBaoSucKhoe.FindAsync(id);
        if (canhBao == null)
            return NotFound();

        canhBao.TrangThai = "Đã xem";
        await _context.SaveChangesAsync();
        return Ok();
    }
}
