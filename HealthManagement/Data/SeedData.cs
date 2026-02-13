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
            await SeedThucDon(context);
            await SeedBaiViet(context);
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

        private static async Task SeedThucDon(ApplicationDbContext context)
        {
            if (!context.ThucDon.Any())
            {
                context.ThucDon.AddRange(
                    new ThucDon
                    {
                        TenMonAn = "Cháo gà",
                        MoTa = "Cháo gà dinh dưỡng, dễ tiêu hóa",
                        Calo = 150,
                        Protein = 12,
                        Carbs = 20,
                        Fat = 3,
                        LoaiMonAn = "Sáng",
                        TrangThai = true
                    },
                    new ThucDon
                    {
                        TenMonAn = "Cơm gạo lứt + Thịt gà luộc + Rau xanh",
                        MoTa = "Bữa trưa healthy, cân bằng dinh dưỡng",
                        Calo = 450,
                        Protein = 35,
                        Carbs = 50,
                        Fat = 10,
                        LoaiMonAn = "Trưa",
                        TrangThai = true
                    },
                    new ThucDon
                    {
                        TenMonAn = "Salad trộn dầu olive",
                        MoTa = "Salad rau củ tươi mát",
                        Calo = 200,
                        Protein = 8,
                        Carbs = 25,
                        Fat = 8,
                        LoaiMonAn = "Tối",
                        TrangThai = true
                    },
                    new ThucDon
                    {
                        TenMonAn = "Sữa chua không đường + Hoa quả",
                        MoTa = "Món phụ bổ sung dinh dưỡng",
                        Calo = 120,
                        Protein = 6,
                        Carbs = 18,
                        Fat = 3,
                        LoaiMonAn = "Phụ",
                        TrangThai = true
                    },
                    new ThucDon
                    {
                        TenMonAn = "Cá hồi nướng + Khoai lang",
                        MoTa = "Bữa tối giàu Omega-3",
                        Calo = 380,
                        Protein = 30,
                        Carbs = 35,
                        Fat = 12,
                        LoaiMonAn = "Tối",
                        TrangThai = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedBaiViet(ApplicationDbContext context)
        {
            if (!context.BaiVietSucKhoe.Any())
            {
                context.BaiVietSucKhoe.AddRange(
                    new BaiVietSucKhoe
                    {
                        TieuDe = "10 Thói quen tốt cho sức khỏe tim mạch",
                        MoTaNgan = "Những thói quen đơn giản giúp bảo vệ tim mạch hiệu quả",
                        NoiDung = @"<h3>1. Tập thể dục đều đặn</h3>
                                   <p>Ít nhất 30 phút mỗi ngày giúp cải thiện tuần hoàn máu.</p>
                                   <h3>2. Ăn nhiều rau xanh và trái cây</h3>
                                   <p>Cung cấp vitamin, chất xơ và chất chống oxi hóa.</p>
                                   <h3>3. Giảm muối và đường</h3>
                                   <p>Hạn chế nguy cơ tăng huyết áp và tiểu đường.</p>",
                        DanhMuc = "Sức khỏe tim mạch",
                        TacGia = "BS. Nguyễn Văn A",
                        NgayDang = DateTime.Now.AddDays(-10),
                        TrangThai = true,
                        Tags = "tim mạch, sức khỏe, phòng bệnh"
                    },
                    new BaiVietSucKhoe
                    {
                        TieuDe = "Chế độ ăn Keto có phù hợp với bạn?",
                        MoTaNgan = "Tìm hiểu về chế độ ăn Keto và những lưu ý quan trọng",
                        NoiDung = @"<h3>Keto là gì?</h3>
                                   <p>Chế độ ăn giảm carb, tăng chất béo lành mạnh.</p>
                                   <h3>Ưu điểm</h3>
                                   <ul><li>Giảm cân hiệu quả</li><li>Cải thiện độ nhạy insulin</li></ul>
                                   <h3>Nhược điểm</h3>
                                   <ul><li>Khó duy trì lâu dài</li><li>Có thể gây mệt mỏi ban đầu</li></ul>",
                        DanhMuc = "Dinh dưỡng",
                        TacGia = "Chuyên gia dinh dưỡng Trần B",
                        NgayDang = DateTime.Now.AddDays(-5),
                        TrangThai = true,
                        Tags = "keto, giảm cân, dinh dưỡng"
                    },
                    new BaiVietSucKhoe
                    {
                        TieuDe = "Yoga cho người mới bắt đầu",
                        MoTaNgan = "Hướng dẫn các tư thế Yoga cơ bản dễ thực hiện",
                        NoiDung = @"<h3>Lợi ích của Yoga</h3>
                                   <p>Tăng sự linh hoạt, giảm stress, cải thiện tư thế.</p>
                                   <h3>5 tư thế cơ bản</h3>
                                   <ol>
                                   <li>Mountain Pose (Tadasana)</li>
                                   <li>Downward Dog (Adho Mukha Svanasana)</li>
                                   <li>Warrior I (Virabhadrasana I)</li>
                                   <li>Tree Pose (Vrksasana)</li>
                                   <li>Child's Pose (Balasana)</li>
                                   </ol>",
                        DanhMuc = "Tập luyện",
                        TacGia = "Huấn luyện viên Yoga Lê C",
                        NgayDang = DateTime.Now.AddDays(-2),
                        TrangThai = true,
                        Tags = "yoga, tập luyện, thư giãn"
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
