using Library.Areas.Admin.Services;
using Library.Config;
using Library.Models;
using Library.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles =AppRole.Admin)]
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoriesManagerService _categoriesManagerService;
        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesManagerService categoriesManagerService) { 
            _logger = logger;
            _categoriesManagerService = categoriesManagerService;
        }
        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _categoriesManagerService.GetCategoriesAsync(page, pageSize);
            if (!response.IsSuccess)
            {
                return NotFound();
            }
            var data = response.Data as dynamic;
            ViewBag.TotalCategories = data?.totalCategories ?? 0;
            ViewBag.TotalPage = data?.totalPages ?? 1;
            ViewBag.currentPageSize = data?.currentPageSize ?? 10;
            ViewBag.CurrentPage = data?.currentPage ?? 1;
            var categories = data?.categories as List<CategoryViewModel>;
            return View(categories);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Show/Category")]
        public async Task<IActionResult> ShowCategory(int id)
        {
            var result = await _categoriesManagerService.ShowCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData["CategoryMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["CategoryMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Hide/Category")]
        public async Task<IActionResult> HideCategory(int id)
        {
            var result = await _categoriesManagerService.HideCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData["CategoryMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["CategoryMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Route("Create/Category")]
        public async Task<IActionResult> CreateCategory(CreateCategory category)
        {
            if (!ModelState.IsValid)
            {
                TempData["CategoryMessage"] = "Tên danh mục không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("Categories");
            }
            var result = await _categoriesManagerService.CreateCategoryAsync(category);
            if (result.IsSuccess)
            {
                TempData["CategoryMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["CategoryMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Update/Category")]
        public async Task<IActionResult> UpdateCategory(int id, string name)
        {
            var result = await _categoriesManagerService.UpdateCategoryAsync(id, name);
            if (result.IsSuccess)
            {
                TempData["CategoryMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["CategoryMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Delete/Category")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoriesManagerService.DeleteCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData["CategoryMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["CategoryMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
    }
}
