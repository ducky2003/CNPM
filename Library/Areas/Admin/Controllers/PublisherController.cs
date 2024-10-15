using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.Areas.Admin.Models;
using Library.Areas.Admin.Services;
using Library.Config;
using Library.Models;
namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRole.Admin)]
    public class PublisherController : Controller
    {
        private readonly ILogger<PublisherController> _logger;
        private readonly IPublisherService _publishManagerService;

        public PublisherController(ILogger<PublisherController> logger, IPublisherService publishManagerService)
        {
            _logger = logger;
            _publishManagerService = publishManagerService;
        }
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _publishManagerService.GetPublishers(page, pageSize);
            if (!response.IsSuccess)
            {
                return StatusCode(500);
            }
            var data = response.Data as dynamic;
            ViewBag.TotalPublishers = data?.totalPublishers;
            ViewBag.TotalPages = data?.totalPages;
            ViewBag.CurrentPage = data?.currentPage;
            ViewBag.CurrentPageSize = data?.currentPageSize;
            return View(data?.publishers as List<PublisherView>);

        }
    }
}
