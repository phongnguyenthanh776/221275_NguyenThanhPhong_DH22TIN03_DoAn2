using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Theo dõi uống nước
    /// </summary>
    [Authorize]
    public class HydrationController : Controller
    {
        private readonly ILifestyleService _lifestyleService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public HydrationController(
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

            var logs = await _lifestyleService.GetUongNuocAsync(user.MaNguoiDung, 7);
            var totals = logs
                .GroupBy(x => x.ThoiGian.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.SoMl));

            var reminders = await _lifestyleService.GetWaterRemindersAsync(user.MaNguoiDung, 7);

            var vm = new HydrationViewModel
            {
                Form = new UongNuoc { SoMl = 250, ThoiGian = DateTime.Now },
                Logs = logs,
                TotalsByDate = totals,
                Reminders = reminders,
                ReminderForm = new NhacUongNuoc { GioNhac = DateTime.Now.AddMinutes(30), SoMl = 250 }
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var record = await _lifestyleService.GetUongNuocByIdAsync(id, user.MaNguoiDung);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new HydrationEditViewModel { Form = record };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(HydrationViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Logs = await _lifestyleService.GetUongNuocAsync(user.MaNguoiDung, 7);
                model.TotalsByDate = model.Logs
                    .GroupBy(x => x.ThoiGian.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.SoMl));
                return View("Index", model);
            }

            model.Form.MaNguoiDung = user.MaNguoiDung;
            await _lifestyleService.AddUongNuocAsync(model.Form);
            TempData["SuccessMessage"] = "Đã lưu lượt uống nước.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HydrationEditViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Form.MaNguoiDung = user.MaNguoiDung;
            var ok = await _lifestyleService.UpdateUongNuocAsync(model.Form, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã cập nhật bản ghi." : "Không tìm thấy bản ghi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _lifestyleService.DeleteUongNuocAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã xoá bản ghi." : "Không tìm thấy bản ghi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReminder(HydrationViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            // Bỏ qua validation của form uống nước khi chỉ gửi form nhắc uống
            ModelState.Remove("Form.SoMl");
            ModelState.Remove("Form.ThoiGian");
            ModelState.Remove("Form.GhiChu");
            ModelState.Remove("Form.MaUongNuoc");
            ModelState.Remove("Form.MaNguoiDung");
            ModelState.Remove("ReminderForm.MaNguoiDung");
            ModelState.Remove("ReminderForm.MaNhac");

            if (!ModelState.IsValid || model.ReminderForm == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu nhắc nhở không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            model.ReminderForm.MaNguoiDung = user.MaNguoiDung;
            await _lifestyleService.AddWaterReminderAsync(model.ReminderForm);
            TempData["SuccessMessage"] = "Đã thêm nhắc uống nước.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReminder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _lifestyleService.ToggleWaterReminderAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã cập nhật nhắc nhở." : "Không tìm thấy nhắc nhở.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var ok = await _lifestyleService.DeleteWaterReminderAsync(id, user.MaNguoiDung);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Đã xoá nhắc nhở." : "Không tìm thấy nhắc nhở.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _userService.GetUserByIdentityIdAsync(identityUser.Id);
        }
    }

    public class HydrationViewModel
    {
        public UongNuoc Form { get; set; } = new UongNuoc();
        public List<UongNuoc> Logs { get; set; } = new List<UongNuoc>();
        public Dictionary<DateTime, int> TotalsByDate { get; set; } = new Dictionary<DateTime, int>();
        public List<NhacUongNuoc> Reminders { get; set; } = new List<NhacUongNuoc>();
        public NhacUongNuoc? ReminderForm { get; set; }
    }

    public class HydrationEditViewModel
    {
        public UongNuoc Form { get; set; } = new UongNuoc();
    }
}
