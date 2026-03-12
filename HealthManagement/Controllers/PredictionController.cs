using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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
        private readonly IWebHostEnvironment _environment;

        public PredictionController(
            IAIService aiService,
            IUserService userService,
            IHealthService healthService,
            UserManager<IdentityUser> userManager,
            ILogger<PredictionController> logger,
            IWebHostEnvironment environment)
        {
            _aiService = aiService;
            _userService = userService;
            _healthService = healthService;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
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
                new { Value = "Stroke", Text = "🧠 Đột Quỵ (Stroke)" },
                new { Value = "KidneyStone", Text = "🪨 Sỏi Thận (Ảnh CT)" },
                new { Value = "Pneumonia", Text = "🫁 Viêm Phổi (Ảnh X-Quang)" }
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
                var huyetApTam = latestMetric.HuyetApTam;
                if (huyetApTam.HasValue)
                {
                    model.AvgBloodPressure = (float)huyetApTam.Value;
                }
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
        // IMAGE-BASED PREDICTION
        // ============================================================

        [HttpGet("Prediction/PredictKidneyStoneImage")]
        public async Task<IActionResult> PredictKidneyStoneImage()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost("Prediction/PredictKidneyStoneImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictKidneyStoneImage(IFormFile? imageFile)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Vui lòng chọn ảnh CT thận.");
                return View();
            }

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var prediction = await _aiService.PredictKidneyStoneImageAsync(imageBytes, imageFile.FileName);
            var storedInput = await BuildStoredImageInputAsync(prediction, nguoiDung.MaNguoiDung, imageFile.FileName, imageFile.Length, "kidney-stone");

            var duDoan = new DuDoanAI
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                LoaiBenhDuDoan = DiseaseType.KidneyStone,
                KetQua = prediction.Result,
                MucDoNguyCo = prediction.RiskLevel,
                ChiTietDuDoan = prediction.Details,
                GoiY = prediction.Recommendation,
                DuLieuDauVao = JsonSerializer.Serialize(storedInput)
            };

            await _userService.SavePredictionAsync(duDoan);
            return View("PredictionResult", prediction);
        }

        [HttpGet("Prediction/PredictPneumoniaImage")]
        public async Task<IActionResult> PredictPneumoniaImage()
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost("Prediction/PredictPneumoniaImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictPneumoniaImage(IFormFile? imageFile)
        {
            var nguoiDung = await GetCurrentUserAsync();
            if (nguoiDung == null) return RedirectToAction("Login", "Account");

            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Vui lòng chọn ảnh X-quang phổi.");
                return View();
            }

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var prediction = await _aiService.PredictPneumoniaImageAsync(imageBytes, imageFile.FileName);
            var storedInput = await BuildStoredImageInputAsync(prediction, nguoiDung.MaNguoiDung, imageFile.FileName, imageFile.Length, "pneumonia");

            var duDoan = new DuDoanAI
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                LoaiBenhDuDoan = DiseaseType.Pneumonia,
                KetQua = prediction.Result,
                MucDoNguyCo = prediction.RiskLevel,
                ChiTietDuDoan = prediction.Details,
                GoiY = prediction.Recommendation,
                DuLieuDauVao = JsonSerializer.Serialize(storedInput)
            };

            await _userService.SavePredictionAsync(duDoan);
            return View("PredictionResult", prediction);
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

            var storedInput = DeserializeStoredPredictionInput(prediction.DuLieuDauVao);
            if (storedInput != null)
            {
                viewModel.OriginalImageBase64 = await LoadStoredImageAsBase64Async(storedInput.OriginalImagePath);
                viewModel.AnnotatedImageBase64 = await LoadStoredImageAsBase64Async(storedInput.AnnotatedImagePath);
                viewModel.VisualizationNote = storedInput.VisualizationNote;
                viewModel.VisualizationMode = storedInput.VisualizationMode;
                viewModel.Probability = storedInput.Probability;
                viewModel.PredictedClass = storedInput.PredictedClass;
                viewModel.DecisionThreshold = storedInput.DecisionThreshold;
                viewModel.ModelType = storedInput.ModelType;
            }

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

        private async Task<StoredPredictionInput> BuildStoredImageInputAsync(
            PredictionResponse prediction,
            int maNguoiDung,
            string fileName,
            long fileSize,
            string diseaseSlug)
        {
            var storedInput = new StoredPredictionInput
            {
                FileName = fileName,
                SizeBytes = fileSize,
                Source = "ImageUpload",
                VisualizationNote = prediction.VisualizationNote,
                VisualizationMode = prediction.VisualizationMode,
                Probability = prediction.Probability,
                PredictedClass = prediction.PredictedClass,
                DecisionThreshold = prediction.DecisionThreshold,
                ModelType = prediction.ModelType
            };

            storedInput.OriginalImagePath = await SaveBase64ImageAsync(prediction.OriginalImageBase64, maNguoiDung, diseaseSlug, "original");
            storedInput.AnnotatedImagePath = await SaveBase64ImageAsync(prediction.AnnotatedImageBase64, maNguoiDung, diseaseSlug, "annotated");

            return storedInput;
        }

        private async Task<string?> SaveBase64ImageAsync(string? base64Image, int maNguoiDung, string diseaseSlug, string suffix)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                return null;
            }

            try
            {
                var bytes = Convert.FromBase64String(base64Image);
                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var folderPath = Path.Combine(webRoot, "uploads", "predictions", maNguoiDung.ToString());
                Directory.CreateDirectory(folderPath);

                var fileName = $"{diseaseSlug}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{suffix}.png";
                var absolutePath = Path.Combine(folderPath, fileName);
                await System.IO.File.WriteAllBytesAsync(absolutePath, bytes);

                return $"/uploads/predictions/{maNguoiDung}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể lưu ảnh phân tích cho lịch sử dự đoán.");
                return null;
            }
        }

        private async Task<string?> LoadStoredImageAsBase64Async(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            var sanitizedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var absolutePath = Path.Combine(webRoot, sanitizedPath);
            if (!System.IO.File.Exists(absolutePath))
            {
                return null;
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
            return Convert.ToBase64String(bytes);
        }

        private static StoredPredictionInput? DeserializeStoredPredictionInput(string? rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<StoredPredictionInput>(rawJson);
            }
            catch
            {
                return null;
            }
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

        private sealed class StoredPredictionInput
        {
            public string? FileName { get; set; }
            public long? SizeBytes { get; set; }
            public string? Source { get; set; }
            public string? OriginalImagePath { get; set; }
            public string? AnnotatedImagePath { get; set; }
            public string? VisualizationNote { get; set; }
            public string? VisualizationMode { get; set; }
            public decimal? Probability { get; set; }
            public int? PredictedClass { get; set; }
            public decimal? DecisionThreshold { get; set; }
            public string? ModelType { get; set; }
        }
    }
}
