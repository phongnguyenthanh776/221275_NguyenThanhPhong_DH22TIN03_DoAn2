using HealthManagement.Models;

namespace HealthManagement.ViewModels
{
    /// <summary>
    /// Dữ liệu hiển thị Dashboard tổng quan.
    /// </summary>
    public class DashboardViewModel
    {
        public NguoiDung NguoiDung { get; set; } = null!;
        public List<LichSuBMI> BMIHistory { get; set; } = new();
        public List<ChiSoSucKhoe> HealthMetrics { get; set; } = new();
        public List<DuDoanAI> PredictionHistory { get; set; } = new();
        public HoSoSucKhoe? HealthProfile { get; set; }
        public ChiSoSucKhoe? LatestMetric { get; set; }
        public List<GiacNgu> SleepLogs { get; set; } = new();
        public List<UongNuoc> WaterLogs { get; set; } = new();
        public decimal? AverageSleepHours { get; set; }
        public decimal? LastSleepHours { get; set; }
        public int TotalWaterMlWeek { get; set; }
        public int TodayWaterMl { get; set; }
        public DateTime? LastWaterLog { get; set; }
    }
}
