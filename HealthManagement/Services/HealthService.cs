using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến sức khỏe
    /// </summary>
    public interface IHealthService
    {
        Task<LichSuBMI> CalculateBMIAsync(int maNguoiDung, decimal chieuCao, decimal canNang);
        Task<List<LichSuBMI>> GetBMIHistoryAsync(int maNguoiDung, int limit = 10);
        Task<HoSoSucKhoe?> GetHealthProfileAsync(int maNguoiDung);
        Task<HoSoSucKhoe> UpdateHealthProfileAsync(HoSoSucKhoe hoSo);
        Task<ChiSoSucKhoe> AddHealthMetricAsync(ChiSoSucKhoe chiSo);
        Task<List<ChiSoSucKhoe>> GetHealthMetricsAsync(int maNguoiDung, int limit = 10);
        Task<ChiSoSucKhoe?> GetLatestHealthMetricAsync(int maNguoiDung);
    }

    public class HealthService : IHealthService
    {
        private readonly ApplicationDbContext _context;

        public HealthService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tính toán BMI và lưu vào lịch sử
        /// </summary>
        public async Task<LichSuBMI> CalculateBMIAsync(int maNguoiDung, decimal chieuCao, decimal canNang)
        {
            // Chuyển chiều cao từ cm sang m
            decimal chieuCaoM = chieuCao / 100;

            // Công thức BMI = Cân nặng (kg) / (Chiều cao (m))^2
            decimal bmi = canNang / (chieuCaoM * chieuCaoM);
            bmi = Math.Round(bmi, 2);

            // Lấy thông tin người dùng để cá nhân hóa lời khuyên
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
            var age = nguoiDung != null ? GetAge(nguoiDung.NgaySinh) : (int?)null;
            var gender = nguoiDung?.GioiTinh;
            var (phanLoai, goiY) = GetBmiAdvice(bmi, gender, age);

            // Phân loại BMI theo WHO
            var lichSu = new LichSuBMI
            {
                MaNguoiDung = maNguoiDung,
                ChieuCao = chieuCao,
                CanNang = canNang,
                BMI = bmi,
                PhanLoai = phanLoai,
                GoiY = goiY,
                NgayTinh = DateTime.Now
            };

            _context.LichSuBMI.Add(lichSu);
            await _context.SaveChangesAsync();

            // Cập nhật HoSoSucKhoe nếu có
            var hoSo = await _context.HoSoSucKhoe.FirstOrDefaultAsync(h => h.MaNguoiDung == maNguoiDung);
            if (hoSo != null)
            {
                hoSo.ChieuCao = chieuCao;
                hoSo.CanNang = canNang;
                hoSo.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return lichSu;
        }

        private static (string phanLoai, string goiY) GetBmiAdvice(decimal bmi, string? gender, int? age)
        {
            string phanLoai;
            string khuyenNghi;

            if (bmi < 18.5m)
            {
                phanLoai = "Thiếu cân";
                khuyenNghi = "Tăng 300-500 kcal/ngày, ưu tiên protein 1.2-1.5 g/kg, chia nhỏ 4-5 bữa, kiểm tra tuyến giáp nếu sụt cân nhanh.";
            }
            else if (bmi < 25m)
            {
                phanLoai = "Bình thường";
                khuyenNghi = "Duy trì cân nặng, 150-300 phút vận động/tuần, đủ đạm và rau xanh, theo dõi BMI mỗi 3 tháng.";
            }
            else if (bmi < 30m)
            {
                phanLoai = "Thừa cân";
                khuyenNghi = "Giảm 300-500 kcal/ngày, hạn chế đường lỏng/đồ ngọt, 150-300 phút cardio + 2 buổi kháng lực/tuần, mục tiêu -0.5 kg/tuần.";
            }
            else
            {
                phanLoai = "Béo phì";
                khuyenNghi = "Tham vấn chuyên gia, giảm 500-700 kcal/ngày, kết hợp kháng lực + cardio, theo dõi huyết áp/đường huyết/mỡ máu, ngủ 7-8 giờ.";
            }

            // Điều chỉnh theo tuổi và giới tính
            var extras = new List<string>();
            if (age.HasValue && age.Value >= 45)
            {
                extras.Add("Kiểm tra huyết áp, đường huyết, mỡ máu định kỳ (>=45 tuổi).");
            }

            var genderNorm = (gender ?? string.Empty).Trim().ToLowerInvariant();
            if (genderNorm == "nữ" && age.HasValue && age.Value >= 50)
            {
                extras.Add("Lưu ý sức khỏe xương sau 50 tuổi, bổ sung canxi/vitamin D theo tư vấn bác sĩ.");
            }
            else if (genderNorm == "nam" && age.HasValue && age.Value >= 40)
            {
                extras.Add("Theo dõi vòng bụng <90 cm để giảm nguy cơ chuyển hóa.");
            }

            if (extras.Count > 0)
            {
                khuyenNghi = string.Join(" ", new[] { khuyenNghi }.Concat(extras));
            }

            return (phanLoai, khuyenNghi);
        }

        private static int GetAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }

        public async Task<List<LichSuBMI>> GetBMIHistoryAsync(int maNguoiDung, int limit = 10)
        {
            return await _context.LichSuBMI
                .Where(ls => ls.MaNguoiDung == maNguoiDung)
                .OrderByDescending(ls => ls.NgayTinh)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<HoSoSucKhoe?> GetHealthProfileAsync(int maNguoiDung)
        {
            return await _context.HoSoSucKhoe
                .Include(h => h.NguoiDung)
                .FirstOrDefaultAsync(h => h.MaNguoiDung == maNguoiDung);
        }

        public async Task<HoSoSucKhoe> UpdateHealthProfileAsync(HoSoSucKhoe hoSo)
        {
            var existing = await _context.HoSoSucKhoe
                .FirstOrDefaultAsync(h => h.MaHoSo == hoSo.MaHoSo || h.MaNguoiDung == hoSo.MaNguoiDung);

            if (existing == null)
            {
                hoSo.NgayCapNhat = DateTime.Now;
                _context.HoSoSucKhoe.Add(hoSo);
            }
            else
            {
                existing.ChieuCao = hoSo.ChieuCao;
                existing.CanNang = hoSo.CanNang;
                existing.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return existing ?? hoSo;
        }

        public async Task<ChiSoSucKhoe> AddHealthMetricAsync(ChiSoSucKhoe chiSo)
        {
            chiSo.NgayDo = DateTime.Now;
            _context.ChiSoSucKhoe.Add(chiSo);
            await _context.SaveChangesAsync();
            return chiSo;
        }

        public async Task<List<ChiSoSucKhoe>> GetHealthMetricsAsync(int maNguoiDung, int limit = 10)
        {
            return await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung)
                .OrderByDescending(cs => cs.NgayDo)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<ChiSoSucKhoe?> GetLatestHealthMetricAsync(int maNguoiDung)
        {
            return await _context.ChiSoSucKhoe
                .Where(cs => cs.MaNguoiDung == maNguoiDung)
                .OrderByDescending(cs => cs.NgayDo)
                .FirstOrDefaultAsync();
        }
    }
}
