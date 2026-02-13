using HealthManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller hiển thị Bài viết sức khỏe
    /// </summary>
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Danh sách bài viết
        /// </summary>
        public async Task<IActionResult> Index(string? keyword)
        {
            var articles = string.IsNullOrEmpty(keyword)
                ? await _articleService.GetPublishedArticlesAsync(50)
                : await _articleService.SearchArticlesAsync(keyword);

            ViewBag.Keyword = keyword;
            return View(articles);
        }

        /// <summary>
        /// Chi tiết bài viết
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null || !article.TrangThai) return NotFound();

            // Tăng lượt xem
            await _articleService.IncrementViewCountAsync(id);

            return View(article);
        }
    }
}
