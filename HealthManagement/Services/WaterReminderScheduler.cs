using HealthManagement.Data;
using HealthManagement.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthManagement.Services
{
    public class WaterReminderScheduler
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private Timer? _timer;

        public WaterReminderScheduler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await CheckAndSendReminders(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private async Task CheckAndSendReminders()
        {
            var now = DateTime.Now;
            var reminders = _context.WaterReminder
                .Where(r => !r.DaUong && r.GioNhac.Year == now.Year && r.GioNhac.Month == now.Month && r.GioNhac.Day == now.Day && r.GioNhac.Hour == now.Hour && r.GioNhac.Minute == now.Minute)
                .Join(_context.NguoiDung, r => r.MaNguoiDung, nd => nd.MaNguoiDung, (r, nd) => new { r, nd })
                .ToList();

            foreach (var item in reminders)
            {
                var subject = $"Nhắc uống nước";
                var body = $"Đến giờ uống nước: {item.r.SoMl ?? 0} ml lúc {item.r.GioNhac:HH:mm}.";
                await _emailService.SendEmailAsync(item.nd.Email, subject, body);
            }
        }
    }
}
