using HealthManagement.Models;
using System.Text.Json;

namespace HealthManagement.Services
{
    /// <summary>
    /// Interface cho AI Service - hỗ trợ dự đoán 4 loại bệnh
    /// </summary>
    public interface IAIService
    {
        Task<PredictionResponse> PredictHeartDiseaseAsync(HeartDiseaseRequest request);
        Task<PredictionResponse> PredictDiabetesAsync(DiabetesRequest request);
        Task<PredictionResponse> PredictHypertensionAsync(HypertensionRequest request);
        Task<PredictionResponse> PredictStrokeAsync(StrokeRequest request);
        Task<PredictionResponse> PredictAsync(DiseaseType diseaseType, Dictionary<string, object> data);
    }

    /// <summary>
    /// AI Service Implementation - gọi Flask API với trained models
    /// </summary>
    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;

        public AIService(IConfiguration configuration, HttpClient httpClient, ILogger<AIService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        // ============================================================
        // Heart Disease Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictHeartDiseaseAsync(HeartDiseaseRequest request)
        {
            _logger.LogInformation("🫀 Predicting Heart Disease...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age },
                { "sex", request.Sex },
                { "chestpaintype", request.ChestPainType ?? 0 },
                { "restingbp", request.RestingBP ?? 0 },
                { "cholesterol", request.Cholesterol ?? 0 },
                { "fastingbs", request.FastingBS ?? 0 },
                { "maxhr", request.MaxHR ?? 0 },
                { "exerciseangina", request.ExerciseAngina ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.HeartDisease, data);
        }

        // ============================================================
        // Diabetes Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictDiabetesAsync(DiabetesRequest request)
        {
            _logger.LogInformation("🩺 Predicting Diabetes...");
            
            var data = new Dictionary<string, object>
            {
                { "pregnancies", request.Pregnancies ?? 0 },
                { "glucose", request.Glucose ?? 0 },
                { "bloodpressure", request.BloodPressure ?? 0 },
                { "skinthickness", request.SkinThickness ?? 0 },
                { "insulin", request.Insulin ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "diabetespedigreefunction", request.DiabetesPedigreeFunction ?? 0 },
                { "age", request.Age ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Diabetes, data);
        }

        // ============================================================
        // Hypertension Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictHypertensionAsync(HypertensionRequest request)
        {
            _logger.LogInformation("🔴 Predicting Hypertension...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age ?? 0 },
                { "gender", request.Gender ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "cholesterol", request.Cholesterol ?? 0 },
                { "systolicbp", request.SystolicBP ?? 0 },
                { "diastolicbp", request.DiastolicBP ?? 0 },
                { "heartrate", request.HeartRate ?? 0 },
                { "smoking", request.Smoking ?? 0 },
                { "alcohol", request.Alcohol ?? 0 },
                { "physicalactivity", request.PhysicalActivity ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Hypertension, data);
        }

        // ============================================================
        // Stroke Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictStrokeAsync(StrokeRequest request)
        {
            _logger.LogInformation("🧠 Predicting Stroke...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age ?? 0 },
                { "gender", request.Gender ?? 0 },
                { "hypertension", request.Hypertension ?? 0 },
                { "heartdisease", request.HeartDisease ?? 0 },
                { "smoking", request.Smoking ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "avgbloodpressure", request.AvgBloodPressure ?? 0 },
                { "glucose", request.Glucose ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Stroke, data);
        }

        // ============================================================
        // Generic Prediction - gọi Flask API
        // ============================================================
        public async Task<PredictionResponse> PredictAsync(DiseaseType diseaseType, Dictionary<string, object> data)
        {
            try
            {
                bool isAIEnabled = _configuration.GetValue<bool>("AISettings:IsEnabled");
                
                if (isAIEnabled)
                {
                    return await CallFlaskAPIAsync(diseaseType, data);
                }

                _logger.LogWarning("⚠️ AI Service is disabled");
                return GenerateFallbackResponse(diseaseType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Prediction error: {ex.Message}");
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
        }

        // ============================================================
        // Flask API Call
        // ============================================================
        private async Task<PredictionResponse> CallFlaskAPIAsync(DiseaseType diseaseType, Dictionary<string, object> data)
        {
            try
            {
                var apiUrl = _configuration["AISettings:ApiUrl"];
                
                _logger.LogInformation($"🔗 Calling Flask API: {apiUrl}");
                _logger.LogInformation($"   Disease: {diseaseType}");
                
                // Map diseaseType to Flask API format
                string diseaseTypeStr = diseaseType switch
                {
                    DiseaseType.HeartDisease => "heart_disease",
                    DiseaseType.Diabetes => "diabetes",
                    DiseaseType.Hypertension => "hypertension",
                    DiseaseType.Stroke => "stroke",
                    _ => "heart_disease"
                };
                
                // Tạo request payload
                var requestPayload = new
                {
                    disease_type = diseaseTypeStr,
                    Data = data
                };
                
                // Gọi API
                var response = await _httpClient.PostAsJsonAsync(apiUrl, requestPayload);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                    
                    if (result != null)
                    {
                        _logger.LogInformation($"✅ Prediction result: {result.Result} ({result.RiskLevel}%)");
                        return result;
                    }
                }
                else
                {
                    _logger.LogWarning($"⚠️ API returned status: {response.StatusCode}");
                }
                
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Flask API error: {ex.Message}");
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
        }

        // ============================================================
        // Fallback Response
        // ============================================================
        private PredictionResponse GenerateFallbackResponse(DiseaseType diseaseType, bool isError = false)
        {
            var diseaseName = diseaseType switch
            {
                DiseaseType.HeartDisease => "Bệnh Tim",
                DiseaseType.Diabetes => "Bệnh Tiểu Đường",
                DiseaseType.Hypertension => "Huyết Áp Cao",
                DiseaseType.Stroke => "Đột Quỵ",
                _ => "Bệnh"
            };
            
            return new PredictionResponse
            {
                DiseaseType = diseaseType.ToString(),
                Result = "AI tạm thời không khả dụng",
                RiskLevel = 0,
                Recommendation = string.Empty,
                Details = isError
                    ? $"Không thể lấy kết quả {diseaseName} do lỗi hệ thống. Vui lòng thử lại sau."
                    : $"AI đang tắt, không thể đánh giá nguy cơ {diseaseName} lúc này."
            };
        }
    }
}
