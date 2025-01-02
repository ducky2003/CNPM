using System.Diagnostics;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.Service;
using Library.Services;
namespace Library.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBooksService _booksService;

        public BookController(ILogger<BookController> logger, IBooksService booksService)
        {
            _logger = logger;
            _booksService = booksService;
        }
        public async Task<IActionResult> Index(int? page, int? pageSize, string? categoryName, string? authorName)
        {
            var decodeCategoryName = HttpUtility.UrlDecode(categoryName);
            var decodeAuthorName = HttpUtility.UrlDecode(authorName);
            var response = await _booksService.GetListBookAsync(page, pageSize, decodeCategoryName, authorName);
            var data = response.Data as dynamic;
            ViewBag.CategoryName = decodeCategoryName ?? null;
            ViewBag.AuthorName = decodeAuthorName;
            ViewBag.TotalBooks = data?.totalBooks;
            ViewBag.TotalPage = data?.totalPages;
            ViewBag.currentPageSize = data?.currentPageSize;
            ViewBag.CurrentPage = data?.currentPage;
            var booksList = data?.books as List<ViewBook>;
            if (booksList == null || booksList.Count == 0)
            {
                return NotFound();
            }
            return View(booksList);
        }
        [HttpGet]
        [Route("{id}/{slug}")]
        public async Task<IActionResult> BookDetails(int id)
        {
            var response = await _booksService.GetBookDetailAsync(id);
            if (!response.IsSuccess)
            {
                return NotFound();
            }
            return View(response.Data as ViewBook);
        }
    }
}
