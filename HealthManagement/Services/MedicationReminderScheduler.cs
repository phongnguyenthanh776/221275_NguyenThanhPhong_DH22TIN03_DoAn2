using HealthManagement.Data;
using HealthManagement.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthManagement.Services
{
    public class MedicationReminderScheduler
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private Timer? _timer;

        public MedicationReminderScheduler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await CheckAndSendReminders(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private async Task CheckAndSendReminders()
        {
            var now = DateTime.Now;
            var reminders = _context.LichUongThuoc
                .Where(l => !l.DaGuiEmail && !l.DaUong && l.GioNhac <= now)
                .Join(_context.NguoiDung, l => l.Thuoc.MaNguoiDung, nd => nd.MaNguoiDung, (l, nd) => new { l, nd })
                .ToList();

            foreach (var r in reminders)
            {
                var subject = $"Nhắc uống thuốc: {r.l.Thuoc.Ten}";
                var body = $"Đến giờ uống thuốc: {r.l.Thuoc.Ten} - Liều: {r.l.Thuoc.LieuDung} {r.l.Thuoc.DonVi} lúc {r.l.GioNhac:HH:mm}.";
                await _emailService.SendEmailAsync(r.nd.Email, subject, body);
                r.l.DaGuiEmail = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
