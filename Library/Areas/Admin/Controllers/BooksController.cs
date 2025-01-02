using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library.Areas.Admin.Models;
using Library.Areas.Admin.Services;
using Library.Config;
using Library.Models;
using Library.Controllers;
using Library.Areas.Admin.Models.Book;
namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRole.Admin)]
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBooksManagerService _booksManagerService;
        private readonly ICategoriesManagerService _categoriesManagerService;
        private readonly IAuthorManagerService _authorsManagerService;
        private readonly IPublisherService _publisherManagerService;

        public BooksController(ILogger<BooksController> logger, IBooksManagerService booksManagerService, ICategoriesManagerService categoriesManagerService, IAuthorManagerService authorsManagerService, IPublisherService publisherManagerService)
        {
            _logger = logger;
            _booksManagerService = booksManagerService;
            _categoriesManagerService = categoriesManagerService;
            _authorsManagerService = authorsManagerService;
            _publisherManagerService = publisherManagerService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _booksManagerService.GetBooksAsync(page, pageSize);
            if (!response.IsSuccess)
            {
                return StatusCode(500);
            }
            var data = response.Data as dynamic;
            ViewBag.TotalBooks = data?.totalBooks ?? 0;
            ViewBag.TotalPage = data?.totalPages ?? 1;
            ViewBag.currentPageSize = data?.currentPageSize ?? 10;
            ViewBag.CurrentPage = data?.currentPage ?? 1;
            var books = data?.books as List<ViewBook>;
            return View(books);
        }

        [HttpGet]
        [Route("Create/Book")]
        public async Task<IActionResult> CreateBook()
        {
            var categories = await _categoriesManagerService.GetListCategoryAsync();
            var authors = await _authorsManagerService.GetListAuthor();
            var publishers = await _publisherManagerService.GetListPublisherAsync();
            if (!categories.IsSuccess || !authors.IsSuccess || !publishers.IsSuccess)
            {
                return StatusCode(500);
            }
            ViewBag.Categories = new SelectList(categories.Data as List<CategoryViewModel>, "CategoryId", "Name");
            ViewBag.Authors = new SelectList(authors.Data as List<AuthorView>, "AuthorId", "Name");
            ViewBag.Publishers = new SelectList(publishers.Data as List<PublisherView>, "PublisherId", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/Book")]
        public async Task<IActionResult> CreateBook(CreateBook book)
        {
            if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Thông tin sách không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("CreateBook");
            }
            var result = await _booksManagerService.CreateBookAsync(book);
            if (!result.IsSuccess)
            {
                TempData["SystemMessage"] = result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Show/Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowBook(int id)
        {
            var result = await _booksManagerService.ShowBookAsync(id);
            if (result.IsSuccess)
            {
                TempData["SystemMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Hide/Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HideBook(int id)
        {
            var result = await _booksManagerService.HideBookAsync(id);
            if (result.IsSuccess)
            {
                TempData["SystemMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Update/Book/{id:int}")]
        public async Task<IActionResult> UpdateBook(int id)
        {
            var response = await _booksManagerService.GetBookByIdAsync(id);
            if (!response.IsSuccess)
            {
                return NotFound();
            }
            var categories = await _categoriesManagerService.GetListCategoryAsync();
            var authors = await _authorsManagerService.GetListAuthor();
            var publishers = await _publisherManagerService.GetListPublisherAsync();
            if (!categories.IsSuccess || !authors.IsSuccess || !publishers.IsSuccess)
            {
                return NotFound();
            }
            var data = response.Data as ViewBook;
            ViewBag.Categories = new SelectList(categories.Data as List<CategoryViewModel>, "CategoryId", "Name", data?.CategoryId);
            ViewBag.Authors = new SelectList(authors.Data as List<AuthorView>, "AuthorId", "Name", data?.AuthorId);
            ViewBag.Publishers = new SelectList(publishers.Data as List<PublisherView>, "PublisherId", "Name", data?.PublisherId);
            return View((data, new UpdateBook()));
        }
        [HttpPost]
        [Route("Update/Book/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook(int id, [Bind(Prefix = "Item2")] UpdateBook book)
        {
            if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Thông tin sách không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("UpdateBook", new { id });
            }
            var result = await _booksManagerService.UpdateBookAsync(id, book);
            if (!result.IsSuccess)
            {
                TempData["SystemMessage"] = result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("UpdateBook", new { id });
            }
            TempData["SystemMessage"] = result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Delete/Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _booksManagerService.DeleteBookAsync(id);
            if (result.IsSuccess)
            {
                TempData["SystemMessage"] = result.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
    }
}
