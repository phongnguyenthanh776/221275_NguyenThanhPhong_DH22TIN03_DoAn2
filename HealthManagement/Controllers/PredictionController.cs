using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller xử lý dự đoán AI - hỗ trợ 4 loại bệnh
    /// Bệnh Tim, Tiểu Đường, Huyết Áp Cao, Đột Quỵ
    /// </summary>
    [Authorize]
    public class PredictionController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IUserService _userService;
        private readonly IHealthService _healthService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PredictionController> _logger;

        public PredictionController(
            IAIService aiService,
            IUserService userService,
            IHealthService healthService,
            UserManager<IdentityUser> userManager,
            ILogger<PredictionController> logger)
        {
            _aiService = aiService;
            _userService = userService;
            _healthService = healthService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Chọn loại bệnh cần dự đoán
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SelectDisease()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            ViewBag.Diseases = new[]
            {
                new { Value = "HeartDisease", Text = "🫀 Bệnh Tim (Heart Disease)" },
                new { Value = "Diabetes", Text = "🩺 Bệnh Tiểu Đường (Diabetes)" },
                new { Value = "Hypertension", Text = "🔴 Huyết Áp Cao (Hypertension)" },
                new { Value = "Stroke", Text = "🧠 Đột Quỵ (Stroke)" }
            };

            return View();
        }

        // ============================================================
        // HEART DISEASE PREDICTION
        // ============================================================
        
        /// <summary>
        /// Dự đoán bệnh tim
        /// </summary>
        [HttpGet("Prediction/PredictHeartDisease")]
        public async Task<IActionResult> PredictHeartDisease()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);
            
            var model = new HeartDiseaseRequest();
            if (latestMetric != null)
            {
                model.RestingBP = latestMetric.HuyetApTam;
                if (latestMetric.Cholesterol.HasValue) model.Cholesterol = latestMetric.Cholesterol.Value;
                if (latestMetric.DuongHuyet.HasValue) model.FastingBS = latestMetric.DuongHuyet.Value;
                model.MaxHR = latestMetric.NhipTim;
            }

            if (nguoiDung.NgaySinh != default)
            {
                model.Age = DateTime.Now.Year - nguoiDung.NgaySinh.Year;
            }

            model.Sex = MapGenderString(nguoiDung.GioiTinh);

            return View(model);
        }

        [HttpPost("Prediction/PredictHeartDisease")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictHeartDisease(HeartDiseaseRequest model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var prediction = await _aiService.PredictHeartDiseaseAsync(model);

                var duDoan = new DuDoanAI
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    LoaiBenhDuDoan = DiseaseType.HeartDisease,
                    KetQua = prediction.Result,
                    MucDoNguyCo = prediction.RiskLevel,
                    ChiTietDuDoan = prediction.Details,
                    GoiY = prediction.Recommendation,
                    DuLieuDauVao = JsonSerializer.Serialize(model)
                };

                var savedPrediction = await _userService.SavePredictionAsync(duDoan);
                return View("PredictionResult", prediction);
            }

            return View(model);
        }

        // ============================================================
        // DIABETES PREDICTION
        // ============================================================
        
        [HttpGet("Prediction/PredictDiabetes")]
        public async Task<IActionResult> PredictDiabetes()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var model = new DiabetesRequest();
            if (nguoiDung.NgaySinh != default)
            {
                model.Age = DateTime.Now.Year - nguoiDung.NgaySinh.Year;
            }

            return View(model);
        }

        [HttpPost("Prediction/PredictDiabetes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictDiabetes(DiabetesRequest model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var prediction = await _aiService.PredictDiabetesAsync(model);

                var duDoan = new DuDoanAI
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    LoaiBenhDuDoan = DiseaseType.Diabetes,
                    KetQua = prediction.Result,
                    MucDoNguyCo = prediction.RiskLevel,
                    ChiTietDuDoan = prediction.Details,
                    GoiY = prediction.Recommendation,
                    DuLieuDauVao = JsonSerializer.Serialize(model)
                };

                var savedPrediction = await _userService.SavePredictionAsync(duDoan);
                return View("PredictionResult", prediction);
            }

            return View(model);
        }

        // ============================================================
        // HYPERTENSION PREDICTION
        // ============================================================
        
        [HttpGet("Prediction/PredictHypertension")]
        public async Task<IActionResult> PredictHypertension()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);
            
            var model = new HypertensionRequest();
            if (latestMetric != null)
            {
                model.SystolicBP = latestMetric.HuyetApTam;
                if (latestMetric.Cholesterol.HasValue) model.Cholesterol = (int)latestMetric.Cholesterol.Value;
                model.HeartRate = latestMetric.NhipTim;
            }

            if (nguoiDung.NgaySinh != default)
            {
                model.Age = DateTime.Now.Year - nguoiDung.NgaySinh.Year;
            }

            model.Gender = MapGenderNumeric(nguoiDung.GioiTinh);

            return View(model);
        }

        [HttpPost("Prediction/PredictHypertension")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictHypertension(HypertensionRequest model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var prediction = await _aiService.PredictHypertensionAsync(model);

                var duDoan = new DuDoanAI
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    LoaiBenhDuDoan = DiseaseType.Hypertension,
                    KetQua = prediction.Result,
                    MucDoNguyCo = prediction.RiskLevel,
                    ChiTietDuDoan = prediction.Details,
                    GoiY = prediction.Recommendation,
                    DuLieuDauVao = JsonSerializer.Serialize(model)
                };

                var savedPrediction = await _userService.SavePredictionAsync(duDoan);
                return View("PredictionResult", prediction);
            }

            return View(model);
        }

        // ============================================================
        // STROKE PREDICTION
        // ============================================================
        
        [HttpGet("Prediction/PredictStroke")]
        public async Task<IActionResult> PredictStroke()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var latestMetric = await _healthService.GetLatestHealthMetricAsync(nguoiDung.MaNguoiDung);
            
            var model = new StrokeRequest();
            if (latestMetric != null)
            {
                model.AvgBloodPressure = (float)latestMetric.HuyetApTam;
                if (latestMetric.DuongHuyet.HasValue) model.Glucose = (float)latestMetric.DuongHuyet.Value;
            }

            if (nguoiDung.NgaySinh != default)
            {
                model.Age = DateTime.Now.Year - nguoiDung.NgaySinh.Year;
            }

            model.Gender = MapGenderNumeric(nguoiDung.GioiTinh);

            return View(model);
        }

        [HttpPost("Prediction/PredictStroke")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictStroke(StrokeRequest model)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var prediction = await _aiService.PredictStrokeAsync(model);

                var duDoan = new DuDoanAI
                {
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    LoaiBenhDuDoan = DiseaseType.Stroke,
                    KetQua = prediction.Result,
                    MucDoNguyCo = prediction.RiskLevel,
                    ChiTietDuDoan = prediction.Details,
                    GoiY = prediction.Recommendation,
                    DuLieuDauVao = JsonSerializer.Serialize(model)
                };

                var savedPrediction = await _userService.SavePredictionAsync(duDoan);
                return View("PredictionResult", prediction);
            }

            return View(model);
        }

        // ============================================================
        // HISTORY & LEGACY
        // ============================================================
        
        /// <summary>
        /// Lịch sử dự đoán của người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var history = await _userService.GetPredictionHistoryAsync(nguoiDung.MaNguoiDung, 20);
            return View(history);
        }

        /// <summary>
        /// Xem chi tiết dự đoán
        /// </summary>
        [HttpGet("Prediction/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            var prediction = await _userService.GetPredictionByIdAsync(id);
            if (prediction == null || prediction.MaNguoiDung != nguoiDung.MaNguoiDung)
                return NotFound();

            var viewModel = new PredictionResponse
            {
                DiseaseType = prediction.LoaiBenhDuDoan.ToString(),
                Result = prediction.KetQua,
                RiskLevel = prediction.MucDoNguyCo,
                Details = prediction.ChiTietDuDoan ?? string.Empty,
                Recommendation = prediction.GoiY ?? string.Empty
            };

            return View("PredictionResult", viewModel);
        }

        /// <summary>
        /// Legacy: Original Predict action (backward compatibility)
        /// </summary>
        [HttpGet("Prediction/Predict")]
        public IActionResult Predict()
        {
            return RedirectToAction("SelectDisease");
        }

        // Helper method
        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _userService.GetUserByIdentityIdAsync(identityUser.Id);
        }

        private static string MapGenderString(string? gioiTinh)
        {
            var g = gioiTinh?.Trim().ToLowerInvariant();
            if (g == "nữ" || g == "female") return "Female";
            if (g == "nam" || g == "male") return "Male";
            return "Male";
        }

        private static int MapGenderNumeric(string? gioiTinh)
        {
            var g = gioiTinh?.Trim().ToLowerInvariant();
            if (g == "nữ" || g == "female") return 0;
            return 1;
        }
    }
}
