using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    public interface IMedicationService
    {
        Task<List<Thuoc>> GetThuocAsync(int maNguoiDung);
        Task<Thuoc?> GetThuocByIdAsync(int id, int maNguoiDung);
        Task<Thuoc> AddThuocAsync(Thuoc thuoc);
        Task<bool> UpdateThuocAsync(Thuoc thuoc, int maNguoiDung);
        Task<bool> DeleteThuocAsync(int id, int maNguoiDung);

        Task<LichUongThuoc> AddLichAsync(LichUongThuoc lich, int maNguoiDung);
        Task<bool> ToggleDaUongAsync(int lichId, int maNguoiDung);
        Task<bool> DeleteLichAsync(int lichId, int maNguoiDung);
    }

    public class MedicationService : IMedicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public MedicationService(ApplicationDbContext context)
        {
            _context = context;
            // EmailService sẽ được inject qua DI
        }
        public MedicationService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<Thuoc>> GetThuocAsync(int maNguoiDung)
        {
            return await _context.Thuoc
                .Include(t => t.LichUongThuoc)
                .Where(t => t.MaNguoiDung == maNguoiDung && t.TrangThai)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<Thuoc?> GetThuocByIdAsync(int id, int maNguoiDung)
        {
            return await _context.Thuoc
                .Include(t => t.LichUongThuoc)
                .FirstOrDefaultAsync(t => t.MaThuoc == id && t.MaNguoiDung == maNguoiDung);
        }

        public async Task<Thuoc> AddThuocAsync(Thuoc thuoc)
        {
            thuoc.NgayTao = DateTime.Now;
            _context.Thuoc.Add(thuoc);
            await _context.SaveChangesAsync();
            return thuoc;
        }

        public async Task<bool> UpdateThuocAsync(Thuoc thuoc, int maNguoiDung)
        {
            var existing = await GetThuocByIdAsync(thuoc.MaThuoc, maNguoiDung);
            if (existing == null) return false;

            existing.Ten = thuoc.Ten;
            existing.LieuDung = thuoc.LieuDung;
            existing.DonVi = thuoc.DonVi;
            existing.SoLanNgay = thuoc.SoLanNgay;
            existing.GhiChu = thuoc.GhiChu;
            existing.TrangThai = thuoc.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteThuocAsync(int id, int maNguoiDung)
        {
            var existing = await GetThuocByIdAsync(id, maNguoiDung);
            if (existing == null) return false;

            _context.Thuoc.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LichUongThuoc> AddLichAsync(LichUongThuoc lich, int maNguoiDung)
        {
            var thuoc = await GetThuocByIdAsync(lich.MaThuoc, maNguoiDung);
            if (thuoc == null) throw new InvalidOperationException("Thuốc không tồn tại hoặc không thuộc người dùng.");

            _context.LichUongThuoc.Add(lich);
            await _context.SaveChangesAsync();
            return lich;
        }

        public async Task<bool> ToggleDaUongAsync(int lichId, int maNguoiDung)
        {
            var lich = await _context.LichUongThuoc
                .Include(l => l.Thuoc)
                .FirstOrDefaultAsync(l => l.MaLichUong == lichId && l.Thuoc!.MaNguoiDung == maNguoiDung);

            if (lich == null) return false;

            // Flip trạng thái trên server để tránh phụ thuộc dữ liệu gửi từ client
            lich.DaUong = !lich.DaUong;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLichAsync(int lichId, int maNguoiDung)
        {
            var lich = await _context.LichUongThuoc
                .Include(l => l.Thuoc)
                .FirstOrDefaultAsync(l => l.MaLichUong == lichId && l.Thuoc!.MaNguoiDung == maNguoiDung);

            if (lich == null) return false;

            _context.LichUongThuoc.Remove(lich);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
