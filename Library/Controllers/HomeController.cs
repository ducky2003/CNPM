using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBooksService _booksService;

        public HomeController(ILogger<HomeController> logger, IBooksService booksService)
        {
            _logger = logger;
            _booksService = booksService;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _booksService.GetListBookAsync(page, pageSize);
            var data = response.Data as dynamic;
            ViewBag.TotalBooks = data?.totalBooks;
            ViewBag.TotalPage = data?.totalPages;
            ViewBag.currentPageSize = data?.currentPageSize;
            ViewBag.CurrentPage = data?.currentPage;
            var booksList = data?.books as List<ViewBook>;
            return View(booksList);
        }
        [HttpGet]
        [Route("My-profile")]
        public IActionResult MyProfile()
        {
            return View();
        }
    }
}
