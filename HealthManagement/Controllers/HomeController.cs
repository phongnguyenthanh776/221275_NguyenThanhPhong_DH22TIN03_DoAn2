using HealthManagement.Models;
using HealthManagement.Services;
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
        private readonly IArticleService _articleService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            IUserService userService,
            IHealthService healthService,
            IArticleService articleService,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _healthService = healthService;
            _articleService = articleService;
            _userManager = userManager;
        }

        /// <summary>
        /// Trang chủ - Hiển thị bài viết mới nhất
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetPublishedArticlesAsync(6);
            return View(articles);
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

            ViewBag.NguoiDung = nguoiDung;
            ViewBag.BMIHistory = bmiHistory;
            ViewBag.HealthMetrics = healthMetrics;
            ViewBag.PredictionHistory = predictionHistory;

            return View();
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
