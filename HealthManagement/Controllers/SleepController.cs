using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Theo dõi giấc ngủ
    /// </summary>
    [Authorize]
    public class SleepController : Controller
    {
        private readonly ILifestyleService _lifestyleService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public SleepController(
            ILifestyleService lifestyleService,
            IUserService userService,
            UserManager<IdentityUser> userManager)
        {
            _lifestyleService = lifestyleService;
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var vm = new SleepViewModel
            {
                Form = new GiacNgu
                {
                    GioNgu = DateTime.Now.AddHours(-8),
                    GioDay = DateTime.Now
                },
                Logs = await _lifestyleService.GetGiacNguAsync(user.MaNguoiDung, 30)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var record = await _lifestyleService.GetGiacNguByIdAsync(id, user.MaNguoiDung);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new SleepEditViewModel { Form = record };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SleepViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (model.Form.GioDay <= model.Form.GioNgu)
            {
                ModelState.AddModelError("Form.GioDay", "Giờ dậy phải sau giờ ngủ");
            }

            if (!ModelState.IsValid)
            {
                model.Logs = await _lifestyleService.GetGiacNguAsync(user.MaNguoiDung, 30);
                return View("Index", model);
            }

            model.Form.MaNguoiDung = user.MaNguoiDung;
            await _lifestyleService.AddGiacNguAsync(model.Form);
            TempData["SuccessMessage"] = "Đã lưu giấc ngủ.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SleepEditViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (model.Form.GioDay <= model.Form.GioNgu)
            {
                ModelState.AddModelError("Form.GioDay", "Giờ dậy phải sau giờ ngủ");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Form.MaNguoiDung = user.MaNguoiDung;
            var ok = await _lifestyleService.UpdateGiacNguAsync(model.Form, user.MaNguoiDung);
            if (!ok)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đã cập nhật giấc ngủ.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _lifestyleService.DeleteGiacNguAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã xoá bản ghi." : "Không tìm thấy bản ghi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _userService.GetUserByIdentityIdAsync(identityUser.Id);
        }
    }

    public class SleepViewModel
    {
        public GiacNgu Form { get; set; } = new GiacNgu();
        public List<GiacNgu> Logs { get; set; } = new List<GiacNgu>();
    }

    public class SleepEditViewModel
    {
        public GiacNgu Form { get; set; } = new GiacNgu();
    }
}
