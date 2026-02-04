using System.Text;
using System.Text.Json;

public class GoogleAIService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    public GoogleAIService(IConfiguration configuration, HttpClient httpClient)
    {
        _apiKey = configuration["GoogleAI:ApiKey"] ?? "";
        _model = configuration["GoogleAI:Model"] ?? "gemini-1.5-flash";
        _httpClient = httpClient;
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_apiKey);
    }

    public string GetModel()
    {
        return _model;
    }

    public Task<string> PhanTichSucKhoeAsync(string thongTin)
    {
        var prompt = $@"Bạn là một bác sĩ AI chuyên tư vấn sức khỏe. 
Dựa trên thông tin sức khỏe sau, hãy phân tích chi tiết và đưa ra khuyến nghị:

{thongTin}

Trả lời tiếng Việt, có tiêu đề:
1. 📊 Đánh giá tổng thể
2. ⚠️ Các vấn đề phát hiện
3. 💡 Khuyến nghị cải thiện
4. 🎯 Mục tiêu 3 tháng";
        return CallGeminiAsync(prompt);
    }

    public Task<string> GoiYThucDonAsync(string thongTin)
    {
        var prompt = $@"Bạn là chuyên gia dinh dưỡng.
Dựa trên thông tin sau, hãy đề xuất thực đơn 1 ngày:

{thongTin}

Yêu cầu:
- 5 bữa: Sáng, Trưa, Chiều, Tối, Phụ
- Món ăn cụ thể
- Tổng calo & dinh dưỡng
- Trả lời tiếng Việt";
        return CallGeminiAsync(prompt);
    }

    public Task<string> GoiYTapLuyenAsync(string thongTin)
    {
        var prompt = $@"Bạn là huấn luyện viên thể dục.
Dựa trên thông tin sau, hãy lập kế hoạch tập luyện 1 tuần:

{thongTin}

Yêu cầu:
- Kế hoạch chi tiết
- Bài tập rõ ràng
- Phù hợp thể trạng
- Trả lời tiếng Việt";
        return CallGeminiAsync(prompt);
    }

    public Task<string> CallGeminiChatAsync(string prompt)
    {
        return CallGeminiAsync(prompt);
    }

    private async Task<string> CallGeminiAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "[NO_KEY] Chưa cấu hình GoogleAI:ApiKey";

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        
        var body = new
        {
            contents = new[]
            {
                new {
                    parts = new[] {
                        new { text = prompt }
                    }
                }
            }
        };

        try
        {
            var json = JsonSerializer.Serialize(body);
            var resp = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var respText = await resp.Content.ReadAsStringAsync();

            Console.WriteLine($"[Gemini] Status: {resp.StatusCode}");

            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Gemini] Error: {respText.Substring(0, Math.Min(200, respText.Length))}");
                
                if ((int)resp.StatusCode == 429)
                    return "[QUOTA] RESOURCE_EXHAUSTED";
                if ((int)resp.StatusCode == 404)
                    return "[ERROR_MODEL] Model không tồn tại. Hãy đổi sang gemini-1.5-flash";
                
                return "[ERROR] " + respText;
            }

            // Parse successful response
            using var doc = JsonDocument.Parse(respText);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            Console.WriteLine($"[Gemini] Success! Response length: {text?.Length ?? 0}");
            return text ?? "Không có phản hồi từ AI";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Gemini] Exception: {ex.Message}");
            return "[ERROR] Không đọc được phản hồi từ Gemini";
        }
    }
}
