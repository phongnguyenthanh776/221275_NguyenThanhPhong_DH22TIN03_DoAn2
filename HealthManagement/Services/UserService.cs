using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến Người dùng
    /// </summary>
    public interface IUserService
    {
        Task<NguoiDung?> GetUserByIdAsync(int maNguoiDung);
        Task<NguoiDung?> GetUserByEmailAsync(string email);
        Task<NguoiDung?> GetUserByIdentityIdAsync(string identityUserId);
        Task<List<NguoiDung>> GetAllUsersAsync();
        Task<NguoiDung> CreateUserAsync(NguoiDung nguoiDung, string password);
        Task<NguoiDung> UpdateUserAsync(NguoiDung nguoiDung);
        Task<bool> DeleteUserAsync(int maNguoiDung);
        Task<List<DuDoanAI>> GetPredictionHistoryAsync(int maNguoiDung, int limit = 10);
        Task<DuDoanAI?> GetPredictionByIdAsync(int maDuDoan);
        Task<DuDoanAI> SavePredictionAsync(DuDoanAI duDoan);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<NguoiDung?> GetUserByIdAsync(int maNguoiDung)
        {
            return await _context.NguoiDung
                .Include(nd => nd.VaiTro)
                .Include(nd => nd.HoSoSucKhoe)
                .FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
        }

        public async Task<NguoiDung?> GetUserByEmailAsync(string email)
        {
            return await _context.NguoiDung
                .Include(nd => nd.VaiTro)
                .FirstOrDefaultAsync(nd => nd.Email == email);
        }

        public async Task<NguoiDung?> GetUserByIdentityIdAsync(string identityUserId)
        {
            return await _context.NguoiDung
                .Include(nd => nd.VaiTro)
                .Include(nd => nd.HoSoSucKhoe)
                .FirstOrDefaultAsync(nd => nd.IdentityUserId == identityUserId);
        }

        public async Task<List<NguoiDung>> GetAllUsersAsync()
        {
            return await _context.NguoiDung
                .Include(nd => nd.VaiTro)
                .OrderByDescending(nd => nd.NgayDangKy)
                .ToListAsync();
        }

        public async Task<NguoiDung> CreateUserAsync(NguoiDung nguoiDung, string password)
        {
            // Tạo Identity User
            var identityUser = new IdentityUser
            {
                UserName = nguoiDung.Email,
                Email = nguoiDung.Email,
                EmailConfirmed = true  // Bỏ qua email confirmation
            };

            var result = await _userManager.CreateAsync(identityUser, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, "NguoiDung");

                nguoiDung.IdentityUserId = identityUser.Id;
                nguoiDung.NgayDangKy = DateTime.Now;

                _context.NguoiDung.Add(nguoiDung);
                await _context.SaveChangesAsync();

                return nguoiDung;
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<NguoiDung> UpdateUserAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDung.Update(nguoiDung);
            await _context.SaveChangesAsync();
            return nguoiDung;
        }

        public async Task<bool> DeleteUserAsync(int maNguoiDung)
        {
            var user = await _context.NguoiDung.FindAsync(maNguoiDung);
            if (user != null)
            {
                user.TrangThai = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DuDoanAI>> GetPredictionHistoryAsync(int maNguoiDung, int limit = 10)
        {
            return await _context.DuDoanAI
                .Where(dd => dd.MaNguoiDung == maNguoiDung)
                .OrderByDescending(dd => dd.NgayDuDoan)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dự đoán theo ID
        /// </summary>
        public async Task<DuDoanAI?> GetPredictionByIdAsync(int maDuDoan)
        {
            return await _context.DuDoanAI
                .Include(dd => dd.NguoiDung)
                .FirstOrDefaultAsync(dd => dd.MaDuDoan == maDuDoan);
        }

        public async Task<DuDoanAI> SavePredictionAsync(DuDoanAI duDoan)
        {
            duDoan.NgayDuDoan = DateTime.Now;
            _context.DuDoanAI.Add(duDoan);
            await _context.SaveChangesAsync();
            return duDoan;
        }
    }
}
