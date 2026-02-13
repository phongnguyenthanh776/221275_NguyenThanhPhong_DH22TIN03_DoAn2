using HealthManagement.Data;
using HealthManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến Bài viết sức khỏe
    /// </summary>
    public interface IArticleService
    {
        Task<List<BaiVietSucKhoe>> GetAllArticlesAsync();
        Task<List<BaiVietSucKhoe>> GetPublishedArticlesAsync(int limit = 10);
        Task<BaiVietSucKhoe?> GetArticleByIdAsync(int maBaiViet);
        Task<BaiVietSucKhoe> CreateArticleAsync(BaiVietSucKhoe baiViet);
        Task<BaiVietSucKhoe> UpdateArticleAsync(BaiVietSucKhoe baiViet);
        Task<bool> DeleteArticleAsync(int maBaiViet);
        Task IncrementViewCountAsync(int maBaiViet);
        Task<List<BaiVietSucKhoe>> SearchArticlesAsync(string keyword);
    }

    public class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext _context;

        public ArticleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BaiVietSucKhoe>> GetAllArticlesAsync()
        {
            return await _context.BaiVietSucKhoe
                .OrderByDescending(bv => bv.NgayDang)
                .ToListAsync();
        }

        public async Task<List<BaiVietSucKhoe>> GetPublishedArticlesAsync(int limit = 10)
        {
            return await _context.BaiVietSucKhoe
                .Where(bv => bv.TrangThai)
                .OrderByDescending(bv => bv.NgayDang)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<BaiVietSucKhoe?> GetArticleByIdAsync(int maBaiViet)
        {
            return await _context.BaiVietSucKhoe.FindAsync(maBaiViet);
        }

        public async Task<BaiVietSucKhoe> CreateArticleAsync(BaiVietSucKhoe baiViet)
        {
            baiViet.NgayDang = DateTime.Now;
            _context.BaiVietSucKhoe.Add(baiViet);
            await _context.SaveChangesAsync();
            return baiViet;
        }

        public async Task<BaiVietSucKhoe> UpdateArticleAsync(BaiVietSucKhoe baiViet)
        {
            _context.BaiVietSucKhoe.Update(baiViet);
            await _context.SaveChangesAsync();
            return baiViet;
        }

        public async Task<bool> DeleteArticleAsync(int maBaiViet)
        {
            var article = await _context.BaiVietSucKhoe.FindAsync(maBaiViet);
            if (article != null)
            {
                article.TrangThai = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task IncrementViewCountAsync(int maBaiViet)
        {
            var article = await _context.BaiVietSucKhoe.FindAsync(maBaiViet);
            if (article != null)
            {
                article.LuotXem++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BaiVietSucKhoe>> SearchArticlesAsync(string keyword)
        {
            return await _context.BaiVietSucKhoe
                .Where(bv => bv.TrangThai && 
                       (bv.TieuDe.Contains(keyword) || 
                        bv.NoiDung.Contains(keyword) ||
                        (bv.Tags != null && bv.Tags.Contains(keyword))))
                .OrderByDescending(bv => bv.NgayDang)
                .ToListAsync();
        }
    }
}
