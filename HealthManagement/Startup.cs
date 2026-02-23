using HealthManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HealthManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddHostedService<ReminderBackgroundService>();
        }
    }
}