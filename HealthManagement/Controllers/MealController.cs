using HealthManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthManagement.Controllers
{
    /// <summary>
    /// Controller hiển thị Thực đơn và Bài viết
    /// </summary>
    public class MealController : Controller
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        /// <summary>
        /// FORM 11: Danh sách thực đơn
        /// </summary>
        public async Task<IActionResult> Index(string? loaiMonAn)
        {
            var meals = string.IsNullOrEmpty(loaiMonAn)
                ? await _mealService.GetAllMealsAsync()
                : await _mealService.GetMealsByTypeAsync(loaiMonAn);

            ViewBag.LoaiMonAn = loaiMonAn;
            return View(meals);
        }

        /// <summary>
        /// Chi tiết món ăn
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null) return NotFound();

            return View(meal);
        }
    }
}
