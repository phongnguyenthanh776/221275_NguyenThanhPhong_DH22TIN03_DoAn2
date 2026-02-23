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
            var reminders = _context.NhacUongNuoc
                .Where(r => !r.DaGuiEmail && !r.DaUong && r.GioNhac <= now)
                .Join(_context.NguoiDung, r => r.MaNguoiDung, nd => nd.MaNguoiDung, (r, nd) => new { r, nd })
                .ToList();

            foreach (var item in reminders)
            {
                var subject = "Nhac uong nuoc";
                var body = $"Den gio uong nuoc: {item.r.SoMl ?? 0} ml luc {item.r.GioNhac:HH:mm}.";
                await _emailService.SendEmailAsync(item.nd.Email, subject, body);
                item.r.DaGuiEmail = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
