using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IHealthAnalyticsService _analyticsService;
        private readonly IUserService _userService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IHealthAnalyticsService analyticsService,
            IUserService userService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Dashboard phân tích xu hướng sức khỏe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var email = User.Identity?.Name;
            if (email == null) return RedirectToAction("Login", "Account");

            var nguoiDung = await _userService.GetUserByEmailAsync(email);
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            // Get 30-day trends
            var trends = await _analyticsService.GetHealthTrendsAsync(nguoiDung.MaNguoiDung, 30);
            
            // Get alerts
            var alerts = await _analyticsService.GetHealthAlertsAsync(nguoiDung.MaNguoiDung);
            
            // Get comparison
            var comparison = await _analyticsService.CompareWithAverageAsync(nguoiDung.MaNguoiDung);

            ViewBag.Trends = trends;
            ViewBag.Alerts = alerts;
            ViewBag.Comparison = comparison;

            return View();
        }

        /// <summary>
        /// Báo cáo sức khỏe hàng tuần
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> WeeklyReport()
        {
            var email = User.Identity?.Name;
            if (email == null) return RedirectToAction("Login", "Account");

            var nguoiDung = await _userService.GetUserByEmailAsync(email);
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var report = await _analyticsService.GenerateWeeklyReportAsync(nguoiDung.MaNguoiDung);

            return View(report);
        }

        /// <summary>
        /// API endpoint: Get health alerts (JSON)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAlertsJson()
        {
            var email = User.Identity?.Name;
            if (email == null) return Json(new { success = false });

            var nguoiDung = await _userService.GetUserByEmailAsync(email);
            if (nguoiDung == null) return Json(new { success = false });

            var alerts = await _analyticsService.GetHealthAlertsAsync(nguoiDung.MaNguoiDung);

            return Json(new
            {
                success = true,
                alerts = alerts,
                count = alerts.Count
            });
        }
    }
}
