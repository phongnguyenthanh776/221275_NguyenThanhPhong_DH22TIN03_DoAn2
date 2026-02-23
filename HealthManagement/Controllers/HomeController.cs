using HealthManagement.Models;
using HealthManagement.Services;
using HealthManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller quản lý trang chủ và thống kê
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHealthService _healthService;
        private readonly ILifestyleService _lifestyleService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            IUserService userService,
            IHealthService healthService,
            ILifestyleService lifestyleService,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _healthService = healthService;
            _lifestyleService = lifestyleService;
            _userManager = userManager;
        }

        /// <summary>
        /// Trang chủ - Hiển thị bài viết mới nhất
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Dashboard - Hiển thị thống kê cá nhân (yêu cầu đăng nhập)
        /// FORM 1: Dashboard
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return RedirectToAction("Login", "Account");

            var nguoiDung = await _userService.GetUserByIdentityIdAsync(identityUser.Id);
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            // Lấy dữ liệu cho dashboard
            var bmiHistory = await _healthService.GetBMIHistoryAsync(nguoiDung.MaNguoiDung, 7);
            var healthMetrics = await _healthService.GetHealthMetricsAsync(nguoiDung.MaNguoiDung, 7);
            var predictionHistory = await _userService.GetPredictionHistoryAsync(nguoiDung.MaNguoiDung, 5);
            var healthProfile = await _healthService.GetHealthProfileAsync(nguoiDung.MaNguoiDung);
            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);
            var sleepLogs = await _lifestyleService.GetGiacNguAsync(nguoiDung.MaNguoiDung, 7);
            var waterLogs = await _lifestyleService.GetUongNuocAsync(nguoiDung.MaNguoiDung, 7);

            var averageSleep = sleepLogs.Any()
                ? Math.Round(sleepLogs.Average(x => x.TongGio), 1)
                : (decimal?)null;

            var lastSleep = sleepLogs.OrderByDescending(x => x.GioNgu).FirstOrDefault();
            var totalWater = waterLogs.Sum(x => x.SoMl);
            var todayWater = waterLogs.Where(x => x.ThoiGian.Date == DateTime.Today).Sum(x => x.SoMl);
            var lastWater = waterLogs.OrderByDescending(x => x.ThoiGian).FirstOrDefault()?.ThoiGian;

            var vm = new DashboardViewModel
            {
                NguoiDung = nguoiDung,
                BMIHistory = bmiHistory,
                HealthMetrics = healthMetrics,
                PredictionHistory = predictionHistory,
                HealthProfile = healthProfile,
                LatestMetric = latestMetric,
                SleepLogs = sleepLogs,
                WaterLogs = waterLogs,
                AverageSleepHours = averageSleep,
                LastSleepHours = lastSleep?.TongGio,
                TotalWaterMlWeek = totalWater,
                TodayWaterMl = todayWater,
                LastWaterLog = lastWater
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
