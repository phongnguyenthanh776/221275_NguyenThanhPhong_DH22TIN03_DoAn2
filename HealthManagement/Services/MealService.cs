using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến Thực đơn và Kế hoạch ăn uống
    /// </summary>
    public interface IMealService
    {
        Task<List<ThucDon>> GetAllMealsAsync();
        Task<List<ThucDon>> GetMealsByTypeAsync(string loaiMonAn);
        Task<ThucDon?> GetMealByIdAsync(int maMonAn);
        Task<ThucDon> CreateMealAsync(ThucDon thucDon);
        Task<ThucDon> UpdateMealAsync(ThucDon thucDon);
        Task<bool> DeleteMealAsync(int maMonAn);
        Task<KeHoachAnUong> CreateMealPlanAsync(KeHoachAnUong keHoach);
        Task<List<KeHoachAnUong>> GetUserMealPlansAsync(int maNguoiDung);
        Task<KeHoachAnUong?> GetMealPlanDetailAsync(int maKeHoach);
    }

    public class MealService : IMealService
    {
        private readonly ApplicationDbContext _context;

        public MealService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ThucDon>> GetAllMealsAsync()
        {
            return await _context.ThucDon
                .Where(td => td.TrangThai)
                .OrderBy(td => td.LoaiMonAn)
                .ThenBy(td => td.TenMonAn)
                .ToListAsync();
        }

        public async Task<List<ThucDon>> GetMealsByTypeAsync(string loaiMonAn)
        {
            return await _context.ThucDon
                .Where(td => td.TrangThai && td.LoaiMonAn == loaiMonAn)
                .OrderBy(td => td.TenMonAn)
                .ToListAsync();
        }

        public async Task<ThucDon?> GetMealByIdAsync(int maMonAn)
        {
            return await _context.ThucDon.FindAsync(maMonAn);
        }

        public async Task<ThucDon> CreateMealAsync(ThucDon thucDon)
        {
            thucDon.NgayTao = DateTime.Now;
            _context.ThucDon.Add(thucDon);
            await _context.SaveChangesAsync();
            return thucDon;
        }

        public async Task<ThucDon> UpdateMealAsync(ThucDon thucDon)
        {
            _context.ThucDon.Update(thucDon);
            await _context.SaveChangesAsync();
            return thucDon;
        }

        public async Task<bool> DeleteMealAsync(int maMonAn)
        {
            var meal = await _context.ThucDon.FindAsync(maMonAn);
            if (meal != null)
            {
                meal.TrangThai = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<KeHoachAnUong> CreateMealPlanAsync(KeHoachAnUong keHoach)
        {
            keHoach.NgayTao = DateTime.Now;
            _context.KeHoachAnUong.Add(keHoach);
            await _context.SaveChangesAsync();
            return keHoach;
        }

        public async Task<List<KeHoachAnUong>> GetUserMealPlansAsync(int maNguoiDung)
        {
            return await _context.KeHoachAnUong
                .Where(kh => kh.MaNguoiDung == maNguoiDung)
                .OrderByDescending(kh => kh.NgayTao)
                .ToListAsync();
        }

        public async Task<KeHoachAnUong?> GetMealPlanDetailAsync(int maKeHoach)
        {
            return await _context.KeHoachAnUong
                .Include(kh => kh.ChiTietKeHoachAn)
                .ThenInclude(ct => ct.ThucDon)
                .FirstOrDefaultAsync(kh => kh.MaKeHoach == maKeHoach);
        }
    }
}
