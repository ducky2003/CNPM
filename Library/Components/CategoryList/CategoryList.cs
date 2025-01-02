using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.Data;
using Library.Services;

namespace Smart_Library.Areas.Admin.Components
{
    [ViewComponent(Name = "CategoryList")]
    public class CategoryList : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IBooksService _booksService;
        public CategoryList(ApplicationDbContext context, IBooksService booksService)
        {
            _context = context;
            _booksService = booksService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? selected)
        {
            var response = await _booksService.GetCategoriesAsync();
            var categories = response.Data as List<CategoryViewModel>;
            return await Task.FromResult((IViewComponentResult)View(categories));
        }
    }
}