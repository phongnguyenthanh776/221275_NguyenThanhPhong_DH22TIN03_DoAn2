using HealthManagement.Models;
using HealthManagement.Services;
using HealthManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller quản lý Admin (Quản lý người dùng, bài viết, thực đơn)
    /// Chỉ Admin mới có quyền truy cập
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IUserService userService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userService = userService;
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Trang Admin Dashboard
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var totalNews = await _context.TinTucSucKhoe.CountAsync();

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalNews = totalNews;

            return View();
        }

        #region Quản lý Người dùng
        
        /// <summary>
        /// FORM 12: Quản lý người dùng (Admin)
        /// </summary>
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            
            // Lấy vai trò thực từ Identity (AspNetUserRoles)
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.IdentityUserId))
                {
                    var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                    if (identityUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        userRoles[user.IdentityUserId] = roles.FirstOrDefault() ?? "NguoiDung";
                    }
                }
            }
            
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "Vô hiệu hóa người dùng thành công!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                user.TrangThai = true;
                await _userService.UpdateUserAsync(user);
                TempData["SuccessMessage"] = "Kích hoạt lại người dùng thành công!";
            }
            return RedirectToAction("Users");
        }

        #endregion

        #region Quản lý Tin tức Sức khỏe
        
        /// <summary>
        /// Danh sách tin tức sức khỏe
        /// </summary>
        public async Task<IActionResult> TinTuc()
        {
            var tinTuc = await _context.TinTucSucKhoe
                .OrderBy(t => t.ThuTu)
                .ThenByDescending(t => t.NgayTao)
                .ToListAsync();
            return View(tinTuc);
        }

        /// <summary>
        /// Form tạo tin tức mới
        /// </summary>
        public IActionResult TaoTinTuc()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTinTuc(TinTucSucKhoe tinTuc)
        {
            if (ModelState.IsValid)
            {
                tinTuc.NgayTao = DateTime.Now;
                _context.TinTucSucKhoe.Add(tinTuc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo tin tức thành công!";
                return RedirectToAction("TinTuc");
            }
            return View(tinTuc);
        }

        /// <summary>
        /// Form chỉnh sửa tin tức
        /// </summary>
        public async Task<IActionResult> SuaTinTuc(int id)
        {
            var tinTuc = await _context.TinTucSucKhoe.FindAsync(id);
            if (tinTuc == null)
            {
                return NotFound();
            }
            return View(tinTuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTinTuc(int id, TinTucSucKhoe tinTuc)
        {
            if (id != tinTuc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tinTuc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tin tức thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TinTucExists(tinTuc.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("TinTuc");
            }
            return View(tinTuc);
        }

        /// <summary>
        /// Xóa tin tức
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> XoaTinTuc(int id)
        {
            var tinTuc = await _context.TinTucSucKhoe.FindAsync(id);
            if (tinTuc != null)
            {
                _context.TinTucSucKhoe.Remove(tinTuc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa tin tức thành công!";
            }
            return RedirectToAction("TinTuc");
        }

        private bool TinTucExists(int id)
        {
            return _context.TinTucSucKhoe.Any(e => e.Id == id);
        }

        #endregion

    }
}
