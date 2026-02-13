using HealthManagement.Models;
using HealthManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller quản lý Admin (Quản lý người dùng, bài viết, thực đơn)
    /// Chỉ Admin mới có quyền truy cập
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly IMealService _mealService;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            IUserService userService,
            IArticleService articleService,
            IMealService mealService,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _articleService = articleService;
            _mealService = mealService;
            _userManager = userManager;
        }

        /// <summary>
        /// Trang Admin Dashboard
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var articles = await _articleService.GetAllArticlesAsync();
            var meals = await _mealService.GetAllMealsAsync();

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalArticles = articles.Count;
            ViewBag.TotalMeals = meals.Count;

            return View();
        }

        #region Quản lý Người dùng
        
        /// <summary>
        /// FORM 12: Quản lý người dùng (Admin)
        /// </summary>
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            
            // Lấy vai trò thực từ Identity (AspNetUserRoles)
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.IdentityUserId))
                {
                    var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                    if (identityUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        userRoles[user.IdentityUserId] = roles.FirstOrDefault() ?? "NguoiDung";
                    }
                }
            }
            
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "Vô hiệu hóa người dùng thành công!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                user.TrangThai = true;
                await _userService.UpdateUserAsync(user);
                TempData["SuccessMessage"] = "Kích hoạt lại người dùng thành công!";
            }
            return RedirectToAction("Users");
        }

        #endregion

        #region Quản lý Bài viết

        /// <summary>
        /// FORM 13: Quản lý bài viết (Admin)
        /// </summary>
        public async Task<IActionResult> Articles()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(BaiVietSucKhoe model)
        {
            if (ModelState.IsValid)
            {
                model.TacGia = User.Identity?.Name ?? "Admin";
                await _articleService.CreateArticleAsync(model);
                TempData["SuccessMessage"] = "Tạo bài viết thành công!";
                return RedirectToAction("Articles");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditArticle(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArticle(BaiVietSucKhoe model)
        {
            if (ModelState.IsValid)
            {
                await _articleService.UpdateArticleAsync(model);
                TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
                return RedirectToAction("Articles");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            TempData["SuccessMessage"] = "Xóa bài viết thành công!";
            return RedirectToAction("Articles");
        }

        #endregion

        #region Quản lý Thực đơn

        /// <summary>
        /// FORM 14: Quản lý thực đơn (Admin)
        /// </summary>
        public async Task<IActionResult> Meals()
        {
            var meals = await _mealService.GetAllMealsAsync();
            return View(meals);
        }

        [HttpGet]
        public IActionResult CreateMeal()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMeal(ThucDon model)
        {
            if (ModelState.IsValid)
            {
                await _mealService.CreateMealAsync(model);
                TempData["SuccessMessage"] = "Tạo món ăn thành công!";
                return RedirectToAction("Meals");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMeal(int id)
        {
            var meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMeal(ThucDon model)
        {
            if (ModelState.IsValid)
            {
                await _mealService.UpdateMealAsync(model);
                TempData["SuccessMessage"] = "Cập nhật món ăn thành công!";
                return RedirectToAction("Meals");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            await _mealService.DeleteMealAsync(id);
            TempData["SuccessMessage"] = "Xóa món ăn thành công!";
            return RedirectToAction("Meals");
        }

        #endregion
    }
}
