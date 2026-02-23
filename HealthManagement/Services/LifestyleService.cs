using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service quản lý các nghiệp vụ lối sống: ngủ, uống nước
    /// </summary>
    public interface ILifestyleService
    {
        Task<List<GiacNgu>> GetGiacNguAsync(int maNguoiDung, int days = 30);
        Task<GiacNgu?> GetGiacNguByIdAsync(int id, int maNguoiDung);
        Task<GiacNgu> AddGiacNguAsync(GiacNgu giacNgu);
        Task<bool> UpdateGiacNguAsync(GiacNgu giacNgu, int maNguoiDung);
        Task<bool> DeleteGiacNguAsync(int id, int maNguoiDung);

        Task<List<UongNuoc>> GetUongNuocAsync(int maNguoiDung, int days = 7);
        Task<UongNuoc?> GetUongNuocByIdAsync(int id, int maNguoiDung);
        Task<UongNuoc> AddUongNuocAsync(UongNuoc uongNuoc);
        Task<bool> UpdateUongNuocAsync(UongNuoc uongNuoc, int maNguoiDung);
        Task<bool> DeleteUongNuocAsync(int id, int maNguoiDung);

        Task<List<NhacUongNuoc>> GetWaterRemindersAsync(int maNguoiDung, int days = 7);
        Task<NhacUongNuoc> AddWaterReminderAsync(NhacUongNuoc reminder);
        Task<bool> ToggleWaterReminderAsync(int id, int maNguoiDung);
        Task<bool> DeleteWaterReminderAsync(int id, int maNguoiDung);
    }

    public class LifestyleService : ILifestyleService
    {
        private readonly ApplicationDbContext _context;

        public LifestyleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GiacNgu>> GetGiacNguAsync(int maNguoiDung, int days = 30)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            return await _context.GiacNgu
                .Where(x => x.MaNguoiDung == maNguoiDung && x.GioNgu >= fromDate)
                .OrderByDescending(x => x.GioNgu)
                .ToListAsync();
        }

        public async Task<GiacNgu?> GetGiacNguByIdAsync(int id, int maNguoiDung)
        {
            return await _context.GiacNgu.FirstOrDefaultAsync(x => x.MaGiacNgu == id && x.MaNguoiDung == maNguoiDung);
        }

        public async Task<GiacNgu> AddGiacNguAsync(GiacNgu giacNgu)
        {
            // Tính tổng giờ ngủ dựa trên giờ dậy - giờ ngủ
            var totalHours = (decimal)(giacNgu.GioDay - giacNgu.GioNgu).TotalHours;
            // Chặn giá trị âm hoặc vượt quá 24 giờ để không vi phạm ràng buộc
            totalHours = Math.Clamp(totalHours, 0m, 24m);
            giacNgu.TongGio = Math.Round(totalHours, 2);

            _context.GiacNgu.Add(giacNgu);
            await _context.SaveChangesAsync();
            return giacNgu;
        }

        public async Task<bool> UpdateGiacNguAsync(GiacNgu giacNgu, int maNguoiDung)
        {
            var existing = await GetGiacNguByIdAsync(giacNgu.MaGiacNgu, maNguoiDung);
            if (existing == null) return false;

            existing.GioNgu = giacNgu.GioNgu;
            existing.GioDay = giacNgu.GioDay;
            existing.ChatLuong = giacNgu.ChatLuong;
            existing.GhiChu = giacNgu.GhiChu;
            var totalHours = (decimal)(giacNgu.GioDay - giacNgu.GioNgu).TotalHours;
            totalHours = Math.Clamp(totalHours, 0m, 24m);
            existing.TongGio = Math.Round(totalHours, 2);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGiacNguAsync(int id, int maNguoiDung)
        {
            var existing = await GetGiacNguByIdAsync(id, maNguoiDung);
            if (existing == null) return false;

            _context.GiacNgu.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UongNuoc>> GetUongNuocAsync(int maNguoiDung, int days = 7)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            return await _context.UongNuoc
                .Where(x => x.MaNguoiDung == maNguoiDung && x.ThoiGian >= fromDate)
                .OrderByDescending(x => x.ThoiGian)
                .ToListAsync();
        }

        public async Task<UongNuoc?> GetUongNuocByIdAsync(int id, int maNguoiDung)
        {
            return await _context.UongNuoc.FirstOrDefaultAsync(x => x.MaUongNuoc == id && x.MaNguoiDung == maNguoiDung);
        }

        public async Task<UongNuoc> AddUongNuocAsync(UongNuoc uongNuoc)
        {
            _context.UongNuoc.Add(uongNuoc);
            await _context.SaveChangesAsync();
            return uongNuoc;
        }

        public async Task<bool> UpdateUongNuocAsync(UongNuoc uongNuoc, int maNguoiDung)
        {
            var existing = await GetUongNuocByIdAsync(uongNuoc.MaUongNuoc, maNguoiDung);
            if (existing == null) return false;

            existing.SoMl = uongNuoc.SoMl;
            existing.ThoiGian = uongNuoc.ThoiGian;
            existing.GhiChu = uongNuoc.GhiChu;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUongNuocAsync(int id, int maNguoiDung)
        {
            var existing = await GetUongNuocByIdAsync(id, maNguoiDung);
            if (existing == null) return false;

            _context.UongNuoc.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NhacUongNuoc>> GetWaterRemindersAsync(int maNguoiDung, int days = 7)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            return await _context.NhacUongNuoc
                .Where(r => r.MaNguoiDung == maNguoiDung && r.GioNhac >= fromDate)
                .OrderBy(r => r.GioNhac)
                .ToListAsync();
        }

        public async Task<NhacUongNuoc> AddWaterReminderAsync(NhacUongNuoc reminder)
        {
            _context.NhacUongNuoc.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder;
        }

        public async Task<bool> ToggleWaterReminderAsync(int id, int maNguoiDung)
        {
            var record = await _context.NhacUongNuoc.FirstOrDefaultAsync(r => r.MaNhac == id && r.MaNguoiDung == maNguoiDung);
            if (record == null) return false;

            var wasDone = record.DaUong;

            // Flip the current status so the UI does not depend on client-provided values
            record.DaUong = !record.DaUong;

            // Khi chuyển sang "Đã uống", tự ghi log uống nước
            if (!wasDone && record.DaUong)
            {
                var soMl = record.SoMl ?? 250; // fallback hợp lý nếu không nhập ml
                var now = DateTime.Now;
                var noteFromReminder = string.IsNullOrWhiteSpace(record.GhiChu)
                    ? $"Ghi nhận từ nhắc uống ({record.GioNhac:HH:mm})"
                    : $"Ghi nhận từ nhắc uống ({record.GioNhac:HH:mm}): {record.GhiChu}";

                // Khi đánh dấu đã uống, đồng bộ vào lịch sử uống nước
                _context.UongNuoc.Add(new UongNuoc
                {
                    MaNguoiDung = record.MaNguoiDung,
                    SoMl = soMl,
                    ThoiGian = now,
                    GhiChu = noteFromReminder
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWaterReminderAsync(int id, int maNguoiDung)
        {
            var record = await _context.NhacUongNuoc.FirstOrDefaultAsync(r => r.MaNhac == id && r.MaNguoiDung == maNguoiDung);
            if (record == null) return false;

            _context.NhacUongNuoc.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
