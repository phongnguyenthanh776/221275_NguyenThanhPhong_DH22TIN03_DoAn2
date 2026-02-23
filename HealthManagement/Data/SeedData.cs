using HealthManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Data
{
    /// <summary>
    /// Class seed dữ liệu ban đầu: Roles, Sample data
    /// </summary>
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Tạo database nếu chưa tồn tại
            await context.Database.MigrateAsync();

            // Seed Roles
            await SeedRoles(roleManager, context);

            // Seed Sample Data  
            await SeedVaiTro(context);
            
            // Tùy chọn: Seed thủ công hoặc sync từ AI Model (đã vô hiệu hóa thực đơn cũ)
            // await SeedThucDon(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            string[] roleNames = { "Admin", "NguoiDung" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedVaiTro(ApplicationDbContext context)
        {
            if (!context.VaiTro.Any())
            {
                context.VaiTro.AddRange(
                    new VaiTro
                    {
                        TenVaiTro = "Admin",
                        MoTa = "Quản trị viên hệ thống"
                    },
                    new VaiTro
                    {
                        TenVaiTro = "NguoiDung",
                        MoTa = "Người dùng thông thường"
                    }
                );
                await context.SaveChangesAsync();
            }
        }

    }
}
