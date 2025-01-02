using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.Data;
using Library.Services;

namespace Library.Areas.Admin.Components
{
    [ViewComponent(Name = "AuthorList")]
    public class AuthorList : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IBooksService _booksService;
        public AuthorList(ApplicationDbContext context, IBooksService booksService)
        {
            _context = context;
            _booksService = booksService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? selected)
        {
            var response = await _booksService.GetAuthorsAsync();
            var categories = response.Data as List<AuthorView>;
            return await Task.FromResult((IViewComponentResult)View(categories));
        }
    }
}