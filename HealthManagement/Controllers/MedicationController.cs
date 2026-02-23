using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    [Authorize]
    public class MedicationController : Controller
    {
        private readonly IMedicationService _medicationService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public MedicationController(IMedicationService medicationService, IUserService userService, UserManager<IdentityUser> userManager)
        {
            _medicationService = medicationService;
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var thuocList = await _medicationService.GetThuocAsync(user.MaNguoiDung);
            return View(thuocList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Thuoc { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Thuoc model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.MaNguoiDung = user.MaNguoiDung;
            await _medicationService.AddThuocAsync(model);
            TempData["SuccessMessage"] = "Đã thêm thuốc.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var thuoc = await _medicationService.GetThuocByIdAsync(id, user.MaNguoiDung);
            if (thuoc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thuốc.";
                return RedirectToAction(nameof(Index));
            }

            return View(thuoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Thuoc model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.MaNguoiDung = user.MaNguoiDung;
            var ok = await _medicationService.UpdateThuocAsync(model, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã cập nhật." : "Không tìm thấy thuốc.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _medicationService.DeleteThuocAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã xoá thuốc." : "Không tìm thấy thuốc.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReminder(LichUongThuoc model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu nhắc nhở không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // Không chuyển đổi múi giờ, giữ nguyên giờ người dùng nhập

            try
            {
                await _medicationService.AddLichAsync(model, user.MaNguoiDung);
                TempData["SuccessMessage"] = "Đã thêm giờ nhắc.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thêm được giờ nhắc.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReminder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _medicationService.ToggleDaUongAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã cập nhật trạng thái." : "Không tìm thấy giờ nhắc.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _medicationService.DeleteLichAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã xoá giờ nhắc." : "Không tìm thấy giờ nhắc.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _userService.GetUserByIdentityIdAsync(identityUser.Id);
        }
    }
}
