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
        /// Redirect /Health to /Health/Profile
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Profile));
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
            
            var vm = new HealthProfileViewModel
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                MaHoSo = hoSo?.MaHoSo ?? 0,
                HoTen = nguoiDung.HoTen,
                NgaySinh = nguoiDung.NgaySinh,
                GioiTinh = nguoiDung.GioiTinh,
                ChieuCao = hoSo?.ChieuCao ?? 0,
                CanNang = hoSo?.CanNang ?? 0
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(HealthProfileViewModel model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin người dùng (họ tên, ngày sinh, giới tính)
                nguoiDung.HoTen = model.HoTen;
                nguoiDung.NgaySinh = model.NgaySinh;
                nguoiDung.GioiTinh = model.GioiTinh;
                await _userService.UpdateUserAsync(nguoiDung);

                // Cập nhật hồ sơ sức khỏe cơ bản
                var hoSo = new HoSoSucKhoe
                {
                    MaHoSo = model.MaHoSo,
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    ChieuCao = model.ChieuCao,
                    CanNang = model.CanNang,
                    NgayCapNhat = DateTime.Now
                };

                await _healthService.UpdateHealthProfileAsync(hoSo);

                TempData["SuccessMessage"] = "Cập nhật hồ sơ sức khỏe thành công! Vào mục Tính BMI để tính và nhận khuyến nghị.";
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
                TempData["SuccessMessage"] = "Thêm chỉ số sức khỏe thành công! Bây giờ bạn có thể thực hiện Dự Đoán AI.";
                return RedirectToAction("MetricHistory");
            }

            return View(model);
        }

        /// <summary>
        /// Redirect từ Health đến AI Prediction (Workflow Integration)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GoToPrediction()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);

            if (latestMetric == null)
            {
                TempData["WarningMessage"] = "Vui lòng nhập chỉ số sức khỏe trước khi dự đoán!";
                return RedirectToAction("AddMetric");
            }

            // Chuyển hướng đến Prediction với pre-filled data
            return RedirectToAction("SelectDisease", "Prediction");
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

    public class HealthProfileViewModel
    {
        public int MaHoSo { get; set; }
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

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
