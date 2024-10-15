using Library.Config;
using Library.Data;
using Library.Utils;
using Library.Entities;
using Library.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Library.Areas.Admin.Models;
using Microsoft.IdentityModel.Tokens;
using Library.Models;
namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRole.Admin)]
    public class AuthorController : Controller
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly IAuthorManagerService _authorManagerService;
        public AuthorController(ILogger<AuthorController> logger, IAuthorManagerService authorManagerService)
        {
            _logger = logger;
            _authorManagerService = authorManagerService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _authorManagerService.GetAuthor(page, pageSize);
            if (!response.IsSuccess)
            {
                return StatusCode(500);
            }
            var data = response.Data as dynamic;
            ViewBag.totalAuthors = data?.totalAuthors ?? 0;
            ViewBag.TotalPages = data?.totalPages ?? 1;
            ViewBag.CurrentPage = data?.currentPage ?? 1;
            ViewBag.CurrentPageSize = data?.currentPageSize ?? 10;
            var author = data?.author as List<AuthorView> ?? new List<AuthorView>();
            return View(author);

        }
    }
}
