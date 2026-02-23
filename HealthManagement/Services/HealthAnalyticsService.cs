using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service phân tích xu hướng sức khỏe và cảnh báo thông minh
    /// </summary>
    public interface IHealthAnalyticsService
    {
        Task<HealthTrendAnalysis> GetHealthTrendsAsync(int maNguoiDung, int days = 30);
        Task<List<HealthAlert>> GetHealthAlertsAsync(int maNguoiDung);
        Task<WeeklyHealthReport> GenerateWeeklyReportAsync(int maNguoiDung);
        Task<HealthComparisonResult> CompareWithAverageAsync(int maNguoiDung);
    }

    public class HealthAnalyticsService : IHealthAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthAnalyticsService> _logger;
        private readonly IAIService _aiService;

        public HealthAnalyticsService(
            ApplicationDbContext context,
            ILogger<HealthAnalyticsService> logger,
            IAIService aiService)
        {
            _context = context;
            _logger = logger;
            _aiService = aiService;
        }

        /// <summary>
        /// Phân tích xu hướng chỉ số sức khỏe trong N ngày
        /// </summary>
        public async Task<HealthTrendAnalysis> GetHealthTrendsAsync(int maNguoiDung, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            var metrics = await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung && cs.NgayDo >= startDate)
                .OrderBy(cs => cs.NgayDo)
                .ToListAsync();

            if (!metrics.Any())
                return new HealthTrendAnalysis { HasData = false };

            var latestMetric = metrics.Last();
            var aiPredictions = await PredictAllDiseasesAsync(maNguoiDung, latestMetric);
            var aiHealthScore = CalculateHealthScore(aiPredictions);

            var analysis = new HealthTrendAnalysis
            {
                HasData = true,
                Period = days,
                TotalRecords = metrics.Count,
                StartDate = metrics.First().NgayDo,
                EndDate = metrics.Last().NgayDo,

                // Blood Pressure Trend
                BloodPressureTrend = CalculateTrend(
                    metrics.Select(m => (m.NgayDo, (double?)m.HuyetApTam)).ToList(),
                    "Huyết áp tâm thu"
                ),

                // Blood Sugar Trend
                BloodSugarTrend = CalculateTrend(
                    metrics.Select(m => (m.NgayDo, m.DuongHuyet.HasValue ? (double?)m.DuongHuyet.Value : null)).ToList(),
                    "Đường huyết"
                ),

                // Cholesterol Trend
                CholesterolTrend = CalculateTrend(
                    metrics.Select(m => (m.NgayDo, m.Cholesterol.HasValue ? (double?)m.Cholesterol.Value : null)).ToList(),
                    "Cholesterol"
                ),

                // Heart Rate Trend
                HeartRateTrend = CalculateTrend(
                    metrics.Select(m => (m.NgayDo, (double?)m.NhipTim)).ToList(),
                    "Nhịp tim"
                ),

                // Overall Health Score (0-100) dựa trên AI risk (không dùng rule)
                OverallHealthScore = aiHealthScore,
                AverageRiskLevel = aiPredictions.Any() ? aiPredictions.Average(p => p.RiskLevel) : 0,
                AiPredictions = aiPredictions
            };

            return analysis;
        }

        /// <summary>
        /// Lấy danh sách cảnh báo sức khỏe
        /// </summary>
        public async Task<List<HealthAlert>> GetHealthAlertsAsync(int maNguoiDung)
        {
            var alerts = new List<HealthAlert>();

            var latestMetric = await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung)
                .OrderByDescending(cs => cs.NgayDo)
                .FirstOrDefaultAsync();

            if (latestMetric == null) return alerts;

            var predictions = await PredictAllDiseasesAsync(maNguoiDung, latestMetric);

            foreach (var prediction in predictions)
            {
                var severity = MapSeverity(prediction.Result);
                var mapping = GetDiseasePresentation(prediction.DiseaseType ?? "");

                alerts.Add(new HealthAlert
                {
                    Severity = severity,
                    Category = mapping.Category,
                    Title = prediction.Result,
                    Message = prediction.Details,
                    Icon = mapping.Icon,
                    ActionRequired = prediction.Recommendation
                });
            }

            return alerts
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.Category)
                .ToList();
        }

        /// <summary>
        /// Tạo báo cáo sức khỏe hàng tuần
        /// </summary>
        public async Task<WeeklyHealthReport> GenerateWeeklyReportAsync(int maNguoiDung)
        {
            var weekStart = DateTime.Now.AddDays(-7);
            var metrics = await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung && cs.NgayDo >= weekStart)
                .OrderBy(cs => cs.NgayDo)
                .ToListAsync();

            var predictions = await _context.DuDoanAI
                .Where(d => d.MaNguoiDung == maNguoiDung && d.NgayDuDoan >= weekStart)
                .ToListAsync();

            var aiPredictions = await GetPredictionsInRangeAsync(maNguoiDung, weekStart, DateTime.Now);
            if (!aiPredictions.Any())
            {
                var latestMetric = metrics.LastOrDefault();
                aiPredictions = await PredictAllDiseasesAsync(maNguoiDung, latestMetric);
            }

            var averageRisk = aiPredictions.Any() ? aiPredictions.Average(p => p.RiskLevel) : 0;

            var report = new WeeklyHealthReport
            {
                WeekStart = weekStart,
                WeekEnd = DateTime.Now,
                TotalHealthChecks = metrics.Count,
                TotalPredictions = predictions.Count,

                AverageBloodPressure = metrics.Any(m => m.HuyetApTam.HasValue)
                    ? $"{metrics.Where(m => m.HuyetApTam.HasValue).Average(m => m.HuyetApTam):#}/{metrics.Where(m => m.HuyetApThan.HasValue).Average(m => m.HuyetApThan):#} mmHg"
                    : "Chưa có dữ liệu",

                AverageBloodSugar = metrics.Any(m => m.DuongHuyet.HasValue)
                    ? $"{metrics.Where(m => m.DuongHuyet.HasValue).Average(m => m.DuongHuyet):F1} mg/dL"
                    : "Chưa có dữ liệu",

                AverageCholesterol = metrics.Any(m => m.Cholesterol.HasValue)
                    ? $"{metrics.Where(m => m.Cholesterol.HasValue).Average(m => m.Cholesterol):F1} mg/dL"
                    : "Chưa có dữ liệu",

                AverageHeartRate = metrics.Any(m => m.NhipTim.HasValue)
                    ? $"{metrics.Where(m => m.NhipTim.HasValue).Average(m => m.NhipTim):#} bpm"
                    : "Chưa có dữ liệu",

                HighestRiskDisease = aiPredictions.Any()
                    ? aiPredictions.OrderByDescending(p => p.RiskLevel).First().DiseaseType ?? "Chưa dự đoán"
                    : "Chưa dự đoán",

                HealthStatus = CalculateWeeklyHealthStatus(aiPredictions),
                AverageRiskLevel = averageRisk,
                Recommendations = GenerateWeeklyRecommendations(aiPredictions),
                AiPredictions = aiPredictions
            };

            return report;
        }

        /// <summary>
        /// So sánh chỉ số của người dùng với mức trung bình
        /// </summary>
        public async Task<HealthComparisonResult> CompareWithAverageAsync(int maNguoiDung)
        {
            var userMetric = await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung)
                .OrderByDescending(cs => cs.NgayDo)
                .FirstOrDefaultAsync();

            if (userMetric == null)
                return new HealthComparisonResult { HasData = false };

            // Get average metrics from all users (last 30 days)
            var avgDate = DateTime.Now.AddDays(-30);
            var allMetrics = await _context.ChiSoSucKhoe
                .Where(cs => cs.NgayDo >= avgDate)
                .ToListAsync();

            var aiPredictions = await PredictAllDiseasesAsync(maNguoiDung, userMetric);
            var userRisk = aiPredictions.Any() ? aiPredictions.Average(p => p.RiskLevel) : 0;

            var riskWindow = await _context.DuDoanAI
                .Where(d => d.NgayDuDoan >= avgDate)
                .ToListAsync();

            var avgRisk = riskWindow.Any() ? riskWindow.Average(r => r.MucDoNguyCo) : 0;
            var highestRiskDisease = riskWindow
                .GroupBy(r => r.LoaiBenhDuDoan)
                .OrderByDescending(g => g.Average(x => x.MucDoNguyCo))
                .Select(g => g.Key.ToString())
                .FirstOrDefault() ?? "Chưa dự đoán";

            var result = new HealthComparisonResult
            {
                HasData = true,
                UserBloodPressure = userMetric.HuyetApTam ?? 0,
                AverageBloodPressure = allMetrics.Any(m => m.HuyetApTam.HasValue)
                    ? (int)allMetrics.Where(m => m.HuyetApTam.HasValue).Average(m => m.HuyetApTam.Value)
                    : 0,

                UserBloodSugar = userMetric.DuongHuyet ?? 0,
                AverageBloodSugar = allMetrics.Any(m => m.DuongHuyet.HasValue)
                    ? allMetrics.Where(m => m.DuongHuyet.HasValue).Average(m => m.DuongHuyet.Value)
                    : 0,

                UserCholesterol = userMetric.Cholesterol ?? 0,
                AverageCholesterol = allMetrics.Any(m => m.Cholesterol.HasValue)
                    ? allMetrics.Where(m => m.Cholesterol.HasValue).Average(m => m.Cholesterol.Value)
                    : 0,

                UserHeartRate = userMetric.NhipTim ?? 0,
                AverageHeartRate = allMetrics.Any(m => m.NhipTim.HasValue)
                    ? (int)allMetrics.Where(m => m.NhipTim.HasValue).Average(m => m.NhipTim.Value)
                    : 0,

                UserRiskLevel = userRisk,
                AverageRiskLevel = avgRisk,
                HighestRiskDisease = highestRiskDisease,
                AiPredictions = aiPredictions
            };

            return result;
        }

        // ============================================================
        // PRIVATE HELPER METHODS
        // ============================================================

        private TrendResult CalculateTrend(List<(DateTime Date, double? Value)> data, string name)
        {
            var validData = data.Where(d => d.Value.HasValue).ToList();
            if (validData.Count < 2)
                return new TrendResult { Name = name, Trend = "Chưa đủ dữ liệu" };

            var firstValue = validData.First().Value!.Value;
            var lastValue = validData.Last().Value!.Value;
            var change = lastValue - firstValue;
            var percentChange = (change / firstValue) * 100;

            return new TrendResult
            {
                Name = name,
                FirstValue = firstValue,
                LastValue = lastValue,
                Change = change,
                PercentChange = percentChange,
                Trend = change > 0 ? "Tăng" : (change < 0 ? "Giảm" : "Ổn định"),
                TrendIcon = change > 0 ? "📈" : (change < 0 ? "📉" : "➡️")
            };
        }

        private int CalculateHealthScore(List<PredictionResponse> predictions)
        {
            if (!predictions.Any()) return 50;

            var averageRisk = predictions.Average(p => (double)p.RiskLevel);
            var score = 100 - averageRisk;
            return (int)Math.Clamp(score, 0, 100);
        }

        private string CalculateWeeklyHealthStatus(List<PredictionResponse> predictions)
        {
            if (!predictions.Any()) return "Chưa có dữ liệu";

            var averageRisk = predictions.Average(p => p.RiskLevel);
            if (averageRisk < 33) return "Tốt ✅";
            if (averageRisk < 66) return "Trung bình ⚠️";
            return "Cần cải thiện 🔴";
        }

        private List<string> GenerateWeeklyRecommendations(List<PredictionResponse> predictions)
        {
            if (!predictions.Any())
                return new List<string> { "🤖 Chưa có dự đoán AI trong tuần. Hãy chạy dự đoán để nhận gợi ý cá nhân hóa." };

            return predictions
                .OrderByDescending(p => p.RiskLevel)
                .Select(p => p.Recommendation)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .Take(4)
                .ToList();
        }

        private async Task<List<PredictionResponse>> PredictAllDiseasesAsync(int maNguoiDung, ChiSoSucKhoe? metric)
        {
            var user = await _context.NguoiDung
                .Include(nd => nd.HoSoSucKhoe)
                .FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);

            if (user == null || metric == null)
                return new List<PredictionResponse>();

            var age = user.NgaySinh != default ? DateTime.Now.Year - user.NgaySinh.Year : 35;
            var genderString = NormalizeGender(user.GioiTinh);
            var genderNumeric = genderString.Equals("Female", StringComparison.OrdinalIgnoreCase) ? 0 : 1;

            var bmi = 0m;
            if (user.HoSoSucKhoe != null && user.HoSoSucKhoe.ChieuCao > 0)
            {
                var heightMeters = user.HoSoSucKhoe.ChieuCao / 100m;
                bmi = Math.Round(user.HoSoSucKhoe.CanNang / (heightMeters * heightMeters), 1);
            }

            var heartRequest = new HeartDiseaseRequest
            {
                Age = age,
                Sex = genderString,
                ChestPainType = 0,
                RestingBP = metric.HuyetApTam ?? 0,
                Cholesterol = metric.Cholesterol ?? 0,
                FastingBS = metric.DuongHuyet ?? 0,
                MaxHR = metric.NhipTim ?? 0,
                ExerciseAngina = 0
            };

            var diabetesRequest = new DiabetesRequest
            {
                Glucose = (float)(metric.DuongHuyet ?? 0),
                BloodPressure = (float)(metric.HuyetApTam ?? 0),
                SkinThickness = 0,
                Insulin = 0,
                BMI = bmi > 0 ? (float)bmi : 0,
                DiabetesPedigreeFunction = 0,
                Age = age
            };

            var hypertensionRequest = new HypertensionRequest
            {
                Age = age,
                Gender = genderNumeric,
                BMI = bmi > 0 ? (float)bmi : 0,
                Cholesterol = metric.Cholesterol.HasValue ? (int)metric.Cholesterol.Value : 0,
                SystolicBP = metric.HuyetApTam ?? 0,
                DiastolicBP = metric.HuyetApThan ?? 0,
                HeartRate = metric.NhipTim ?? 0,
                Smoking = 0,
                Alcohol = 0,
                PhysicalActivity = 0
            };

            var strokeRequest = new StrokeRequest
            {
                Age = age,
                Gender = genderNumeric,
                Hypertension = 0,
                HeartDisease = 0,
                Smoking = 0,
                BMI = bmi > 0 ? (float)bmi : 0,
                AvgBloodPressure = metric.HuyetApTam.HasValue ? (float)metric.HuyetApTam.Value : 0,
                Glucose = (float)(metric.DuongHuyet ?? 0)
            };

            var tasks = new List<Task<PredictionResponse>>
            {
                _aiService.PredictHeartDiseaseAsync(heartRequest),
                _aiService.PredictDiabetesAsync(diabetesRequest),
                _aiService.PredictHypertensionAsync(hypertensionRequest),
                _aiService.PredictStrokeAsync(strokeRequest)
            };

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        private static string NormalizeGender(string? gioiTinh)
        {
            var g = gioiTinh?.Trim().ToLowerInvariant();
            if (g == "nữ" || g == "female") return "Female";
            if (g == "nam" || g == "male") return "Male";
            return "Male";
        }

        private async Task<List<PredictionResponse>> GetPredictionsInRangeAsync(int maNguoiDung, DateTime start, DateTime end)
        {
            return await _context.DuDoanAI
                .Where(d => d.MaNguoiDung == maNguoiDung && d.NgayDuDoan >= start && d.NgayDuDoan <= end)
                .OrderByDescending(d => d.NgayDuDoan)
                .Select(d => new PredictionResponse
                {
                    DiseaseType = d.LoaiBenhDuDoan.ToString(),
                    Result = d.KetQua,
                    RiskLevel = d.MucDoNguyCo,
                    Recommendation = d.GoiY ?? string.Empty,
                    Details = d.ChiTietDuDoan ?? string.Empty
                })
                .ToListAsync();
        }

        private AlertSeverity MapSeverity(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return AlertSeverity.Low;

            var normalized = result.ToLowerInvariant();

            if (normalized.Contains("cao")) return AlertSeverity.High;
            if (normalized.Contains("trung")) return AlertSeverity.Medium;
            return AlertSeverity.Low;
        }

        private (string Category, string Icon) GetDiseasePresentation(string diseaseType)
        {
            var normalized = diseaseType.ToLowerInvariant();

            return normalized switch
            {
                "heartdisease" or "heart_disease" => ("Tim mạch", "🫀"),
                "diabetes" => ("Tiểu đường", "🩺"),
                "hypertension" => ("Huyết áp", "🔴"),
                "stroke" => ("Đột quỵ", "🧠"),
                _ => ("Sức khỏe", "⚕️")
            };
        }
    }

    // ============================================================
    // VIEW MODELS
    // ============================================================

    public class HealthTrendAnalysis
    {
        public bool HasData { get; set; }
        public int Period { get; set; }
        public int TotalRecords { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TrendResult BloodPressureTrend { get; set; } = new();
        public TrendResult BloodSugarTrend { get; set; } = new();
        public TrendResult CholesterolTrend { get; set; } = new();
        public TrendResult HeartRateTrend { get; set; } = new();
        public int OverallHealthScore { get; set; }
        public decimal AverageRiskLevel { get; set; }
        public List<PredictionResponse> AiPredictions { get; set; } = new();
    }

    public class TrendResult
    {
        public string Name { get; set; } = "";
        public double FirstValue { get; set; }
        public double LastValue { get; set; }
        public double Change { get; set; }
        public double PercentChange { get; set; }
        public string Trend { get; set; } = "";
        public string TrendIcon { get; set; } = "";
    }

    public class HealthAlert
    {
        public AlertSeverity Severity { get; set; }
        public string Category { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Icon { get; set; } = "";
        public string ActionRequired { get; set; } = "";
    }

    public enum AlertSeverity
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public class WeeklyHealthReport
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalHealthChecks { get; set; }
        public int TotalPredictions { get; set; }
        public string AverageBloodPressure { get; set; } = "";
        public string AverageBloodSugar { get; set; } = "";
        public string AverageCholesterol { get; set; } = "";
        public string AverageHeartRate { get; set; } = "";
        public string HighestRiskDisease { get; set; } = "";
        public string HealthStatus { get; set; } = "";
        public decimal AverageRiskLevel { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public List<PredictionResponse> AiPredictions { get; set; } = new();
    }

    public class HealthComparisonResult
    {
        public bool HasData { get; set; }
        public int UserBloodPressure { get; set; }
        public int AverageBloodPressure { get; set; }
        public decimal UserBloodSugar { get; set; }
        public decimal AverageBloodSugar { get; set; }
        public decimal UserCholesterol { get; set; }
        public decimal AverageCholesterol { get; set; }
        public int UserHeartRate { get; set; }
        public int AverageHeartRate { get; set; }
        public decimal UserRiskLevel { get; set; }
        public decimal AverageRiskLevel { get; set; }
        public string HighestRiskDisease { get; set; } = string.Empty;
        public List<PredictionResponse> AiPredictions { get; set; } = new();
    }
}
