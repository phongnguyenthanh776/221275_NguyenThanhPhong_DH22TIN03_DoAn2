using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers.API
{
    /// <summary>
    /// API Controller cho AI Prediction
    /// CHÚ Ý: Hiện tại endpoint này trả dữ liệu giả lập
    /// Sau khi train model từ Kaggle, đây sẽ là bridge để gọi Flask API
    /// 
    /// ENDPOINT: POST /api/predict
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PredictController : ControllerBase
    {
        private readonly IAIService _aiService;

        public PredictController(IAIService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// API endpoint để dự đoán nguy cơ sức khỏe
        /// Hiện tại: Trả dữ liệu giả lập
        /// Tương lai: Gọi Flask API với trained ML model
        /// 
        /// Request body example:
        /// {
        ///   "age": 55,
        ///   "sex": "Nam",
        ///   "restingBP": 140,
        ///   "cholesterol": 240,
        ///   "fastingBS": 120,
        ///   "maxHR": 150
        /// }
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PredictionResponse>> Predict([FromBody] HeartDiseaseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var prediction = await _aiService.PredictHeartDiseaseAsync(request);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.Now,
                aiEnabled = false,
                message = "API is running. AI inference is currently simulated. Train Kaggle model to enable real predictions."
            });
        }
    }
}
