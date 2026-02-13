using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller quản lý Hồ sơ sức khỏe, Chỉ số, BMI
    /// </summary>
    [Authorize]
    public class HealthController : Controller
    {
        private readonly IHealthService _healthService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public HealthController(
            IHealthService healthService,
            IUserService userService,
            UserManager<IdentityUser> userManager)
        {
            _healthService = healthService;
            _userService = userService;
            _userManager = userManager;
        }

        /// <summary>
        /// FORM 4: Cập nhật hồ sơ sức khỏe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var hoSo = await _healthService.GetHealthProfileAsync(nguoiDung.MaNguoiDung);
            
            if (hoSo == null)
            {
                hoSo = new HoSoSucKhoe
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung
                };
            }

            return View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(HoSoSucKhoe model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            model.MaNguoiDung = nguoiDung.MaNguoiDung;

            if (ModelState.IsValid)
            {
                await _healthService.UpdateHealthProfileAsync(model);
                TempData["SuccessMessage"] = "Cập nhật hồ sơ sức khỏe thành công!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        /// <summary>
        /// FORM 5: Nhập chỉ số sức khỏe
        /// </summary>
        [HttpGet]
        public IActionResult AddMetric()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMetric(ChiSoSucKhoe model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            model.MaNguoiDung = nguoiDung.MaNguoiDung;

            if (ModelState.IsValid)
            {
                await _healthService.AddHealthMetricAsync(model);
                TempData["SuccessMessage"] = "Thêm chỉ số sức khỏe thành công!";
                return RedirectToAction("MetricHistory");
            }

            return View(model);
        }

        /// <summary>
        /// FORM 6: Xem lịch sử chỉ số
        /// </summary>
        public async Task<IActionResult> MetricHistory()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var metrics = await _healthService.GetHealthMetricsAsync(nguoiDung.MaNguoiDung, 20);
            return View(metrics);
        }

        /// <summary>
        /// FORM 7: Tính BMI
        /// </summary>
        [HttpGet]
        public IActionResult CalculateBMI()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateBMI(BMIViewModel model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var result = await _healthService.CalculateBMIAsync(
                    nguoiDung.MaNguoiDung,
                    model.ChieuCao,
                    model.CanNang);

                return View("BMIResult", result);
            }

            return View(model);
        }

        /// <summary>
        /// FORM 8: Lịch sử BMI
        /// </summary>
        public async Task<IActionResult> BMIHistory()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var history = await _healthService.GetBMIHistoryAsync(nguoiDung.MaNguoiDung, 20);
            return View(history);
        }

        // Helper method
        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _userService.GetUserByIdentityIdAsync(identityUser.Id);
        }
    }

    // ViewModel
    public class BMIViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập chiều cao")]
        [Range(30, 250, ErrorMessage = "Chiều cao phải từ 30-250 cm")]
        [Display(Name = "Chiều cao (cm)")]
        public decimal ChieuCao { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập cân nặng")]
        [Range(10, 300, ErrorMessage = "Cân nặng phải từ 10-300 kg")]
        [Display(Name = "Cân nặng (kg)")]
        public decimal CanNang { get; set; }
    }
}
