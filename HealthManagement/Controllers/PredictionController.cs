using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller xử lý Dự đoán AI (hiện tại giả lập)
    /// </summary>
    [Authorize]
    public class PredictionController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IUserService _userService;
        private readonly IHealthService _healthService;
        private readonly UserManager<IdentityUser> _userManager;

        public PredictionController(
            IAIService aiService,
            IUserService userService,
            IHealthService healthService,
            UserManager<IdentityUser> userManager)
        {
            _aiService = aiService;
            _userService = userService;
            _healthService = healthService;
            _userManager = userManager;
        }

        /// <summary>
        /// FORM 9: Form dự đoán nguy cơ sức khỏe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Predict()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            // Lấy chỉ số mới nhất để gợi ý
            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);
            
            var model = new PredictionRequest();
            if (latestMetric != null)
            {
                model.RestingBP = latestMetric.HuyetApTam;
                model.Cholesterol = latestMetric.Cholesterol;
                model.FastingBS = latestMetric.DuongHuyet;
                model.MaxHR = latestMetric.NhipTim;
            }

            // Tính tuổi từ ngày sinh
            if (nguoiDung.NgaySinh != default)
            {
                model.Age = DateTime.Now.Year - nguoiDung.NgaySinh.Year;
            }

            model.Sex = nguoiDung.GioiTinh;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(PredictionRequest model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Gọi AI Service (hiện tại trả dữ liệu giả lập)
                var prediction = await _aiService.PredictHealthRiskAsync(model);

                // Lưu kết quả dự đoán
                var duDoan = new DuDoanAI
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    LoaiDuDoan = "Heart Disease Risk",
                    KetQua = prediction.Result,
                    MucDoNguyCo = prediction.RiskLevel,
                    ChiTietDuDoan = prediction.Details,
                    GoiY = prediction.Recommendation,
                    DuLieuDauVao = JsonSerializer.Serialize(model)
                };

                await _userService.SavePredictionAsync(duDoan);

                return View("PredictionResult", prediction);
            }

            return View(model);
        }

        /// <summary>
        /// FORM 10: Lịch sử dự đoán
        /// </summary>
        public async Task<IActionResult> History()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var history = await _userService.GetPredictionHistoryAsync(nguoiDung.MaNguoiDung, 20);
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
}
