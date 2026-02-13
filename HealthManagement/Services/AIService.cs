using HealthManagement.Models;

namespace HealthManagement.Services
{
    /// <summary>
    /// Interface cho AI Service
    /// CHÚ Ý: Hiện tại trả dữ liệu giả lập
    /// Sau khi train model từ Kaggle dataset (Heart Disease/Diabetes), 
    /// sẽ thay thế bằng gọi Flask API endpoint
    /// 
    /// QUY TRÌNH SAU NÀY:
    /// 1. Download Kaggle dataset (Heart Disease hoặc Diabetes)
    /// 2. Train model bằng Python (Logistic Regression / Random Forest)
    /// 3. Đánh giá Accuracy
    /// 4. Lưu model thành file .pkl
    /// 5. Tạo Flask API POST /predict
    /// 6. Thay code trong PredictHealthRiskAsync để gọi Flask API
    /// </summary>
    public interface IAIService
    {
        Task<PredictionResponse> PredictHealthRiskAsync(PredictionRequest request);
    }

    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Dự đoán nguy cơ sức khỏe
        /// HIỆN TẠI: Trả dữ liệu giả lập dựa trên rule-based
        /// SAU NÀY: Gọi Flask API với trained model
        /// </summary>
        public async Task<PredictionResponse> PredictHealthRiskAsync(PredictionRequest request)
        {
            // Kiểm tra xem AI API có được bật không
            bool isAIEnabled = _configuration.GetValue<bool>("AISettings:IsEnabled");

            if (isAIEnabled)
            {
                // ✅ GỌI FLASK API với trained model từ Kaggle
                return await CallFlaskAPIAsync(request);
            }

            // Fallback: Logic giả lập nếu AI API không khả dụng
            return await Task.FromResult(GenerateMockPrediction(request));
        }

        /// <summary>
        /// Tạo dự đoán giả lập dựa trên các chỉ số
        /// </summary>
        private PredictionResponse GenerateMockPrediction(PredictionRequest request)
        {
            var response = new PredictionResponse();
            int riskScore = 0;

            // Đánh giá các yếu tố nguy cơ
            if (request.Age > 50) riskScore += 20;
            else if (request.Age > 40) riskScore += 10;

            if (request.RestingBP > 140) riskScore += 25;
            else if (request.RestingBP > 120) riskScore += 15;

            if (request.Cholesterol > 240) riskScore += 25;
            else if (request.Cholesterol > 200) riskScore += 15;

            if (request.FastingBS > 125) riskScore += 20;

            if (request.MaxHR < 100) riskScore += 15;

            // Xác định mức độ nguy cơ
            if (riskScore >= 60)
            {
                response.Result = "Nguy cơ cao";
                response.RiskLevel = 75 + (riskScore - 60) / 2;
                response.Recommendation = "Khuyến nghị gặp bác sĩ khám chuyên khoa ngay. Cần kiểm tra sức khỏe toàn diện.";
                response.Details = "Nhiều chỉ số vượt ngưỡng an toàn. Cần theo dõi sát sao và điều chỉnh lối sống.";
            }
            else if (riskScore >= 30)
            {
                response.Result = "Nguy cơ trung bình";
                response.RiskLevel = 40 + (riskScore - 30);
                response.Recommendation = "Nên cải thiện lối sống: ăn uống lành mạnh, tập thể dục đều đặn, giảm stress.";
                response.Details = "Một số chỉ số cần chú ý. Theo dõi định kỳ và duy trì thói quen tốt.";
            }
            else
            {
                response.Result = "Nguy cơ thấp";
                response.RiskLevel = Math.Max(10, riskScore);
                response.Recommendation = "Tiếp tục duy trì lối sống lành mạnh. Khám sức khỏe định kỳ 6 tháng/lần.";
                response.Details = "Các chỉ số sức khỏe trong giới hạn bình thường. Tốt!";
            }

            return response;
        }

        /// <summary>
        /// Gọi Flask API để dự đoán với trained model từ Kaggle dataset
        /// </summary>
        private async Task<PredictionResponse> CallFlaskAPIAsync(PredictionRequest request)
        {
            try
            {
                var apiUrl = _configuration["AISettings:ApiUrl"];
                
                // Gọi Flask API
                var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
                response.EnsureSuccessStatusCode();
                
                // Parse kết quả
                var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                
                // Nếu API trả null hoặc lỗi, fallback sang mock
                return result ?? GenerateMockPrediction(request);
            }
            catch (Exception ex)
            {
                // Log error (có thể thêm ILogger sau)
                Console.WriteLine($"❌ AI API Error: {ex.Message}");
                
                // Fallback: Dùng logic giả lập khi API không khả dụng
                return GenerateMockPrediction(request);
            }
        }
    }
}
