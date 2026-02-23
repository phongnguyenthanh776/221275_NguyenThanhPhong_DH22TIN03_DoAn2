using HealthManagement.Data;
using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthManagement.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5); // Kiem tra nhanh de gui dung gio

        public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.Now;

                    // Gửi nhắc uống thuốc
                    var reminders = db.LichUongThuoc
                        .Include(l => l.Thuoc)
                        .ThenInclude(t => t!.NguoiDung)
                        .Where(l => !l.DaGuiEmail && !l.DaUong && l.GioNhac <= now)
                        .ToList();

                    foreach (var lich in reminders)
                    {
                        if (lich == null || lich.Thuoc == null)
                        {
                            _logger.LogWarning("Bỏ qua lịch nhắc thuốc vì thiếu thông tin thuốc.");
                            continue;
                        }

                        var user = lich.Thuoc.NguoiDung;
                        var email = user?.Email;
                        try
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                await emailService.SendEmailAsync(
                                    email,
                                    $"Nhắc uống thuốc: {lich.Thuoc.Ten}",
                                    $"Đã đến giờ uống thuốc: {lich.Thuoc.Ten} ({lich.Thuoc.LieuDung} {lich.Thuoc.DonVi}) lúc {lich.GioNhac:HH:mm dd/MM/yyyy}."
                                );
                                _logger.LogInformation("Đã gửi mail nhắc uống thuốc cho {Email}", email);
                                lich.DaGuiEmail = true;
                            }
                            else
                            {
                                _logger.LogWarning("Không gửi được mail nhắc thuốc vì user {MaNguoiDung} chưa có email.", lich.Thuoc.MaNguoiDung);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi gửi mail nhắc uống thuốc cho {Email}", email);
                        }
                    }

                    // Gửi nhắc uống nước
                    var waterReminders = db.NhacUongNuoc
                        .Include(n => n.NguoiDung)
                        .Where(n => !n.DaGuiEmail && !n.DaUong && n.GioNhac <= now)
                        .ToList();

                    foreach (var nhac in waterReminders)
                    {
                        var user = nhac.NguoiDung;
                        var email = user?.Email;
                        try
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                await emailService.SendEmailAsync(
                                    email,
                                    "Nhắc uống nước",
                                    $"Đã đến giờ uống nước lúc {nhac.GioNhac:HH:mm dd/MM/yyyy}. Hãy uống đủ nước để bảo vệ sức khỏe!"
                                );
                                _logger.LogInformation("Đã gửi mail nhắc uống nước cho {Email}", email);
                                nhac.DaGuiEmail = true;
                            }
                            else
                            {
                                _logger.LogWarning("Không gửi được mail nhắc nước vì user {MaNguoiDung} chưa có email.", nhac.MaNguoiDung);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi gửi mail nhắc uống nước cho {Email}", email);
                        }
                    }

                    await db.SaveChangesAsync();
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
