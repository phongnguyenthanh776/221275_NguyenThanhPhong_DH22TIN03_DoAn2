using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public AdminController(
            IUserService userService,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        /// <summary>
        /// Trang Admin Dashboard
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();

            ViewBag.TotalUsers = users.Count;

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

    }
}
