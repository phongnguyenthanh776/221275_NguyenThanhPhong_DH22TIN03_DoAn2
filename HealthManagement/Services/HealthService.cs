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

            // Phân loại BMI theo WHO
            string phanLoai;
            string goiY;

            if (bmi < 18.5m)
            {
                phanLoai = "Thiếu cân";
                goiY = "Bạn cần tăng cân. Ăn nhiều bữa nhỏ trong ngày, tăng protein và carbs lành mạnh.";
            }
            else if (bmi < 25m)
            {
                phanLoai = "Bình thường";
                goiY = "Cân nặng của bạn trong giới hạn lý tưởng. Hãy duy trì lối sống lành mạnh!";
            }
            else if (bmi < 30m)
            {
                phanLoai = "Thừa cân";
                goiY = "Bạn có nguy cơ thừa cân. Nên tăng cường vận động và điều chỉnh chế độ ăn.";
            }
            else
            {
                phanLoai = "Béo phì";
                goiY = "Cân nặng cao hơn khuyến nghị. Nên tham khảo ý kiến bác sĩ và chuyên gia dinh dưỡng.";
            }

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
            var existing = await _context.HoSoSucKhoe.FindAsync(hoSo.MaHoSo);

            if (existing == null)
            {
                hoSo.NgayCapNhat = DateTime.Now;
                _context.HoSoSucKhoe.Add(hoSo);
            }
            else
            {
                existing.ChieuCao = hoSo.ChieuCao;
                existing.CanNang = hoSo.CanNang;
                existing.NhomMau = hoSo.NhomMau;
                existing.TienSuBenhLy = hoSo.TienSuBenhLy;
                existing.DiUng = hoSo.DiUng;
                existing.ThuocDangDung = hoSo.ThuocDangDung;
                existing.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return hoSo;
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
