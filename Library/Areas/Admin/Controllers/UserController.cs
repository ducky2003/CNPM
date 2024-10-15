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
namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRole.Admin)]
    public class UserController : Controller
    {
        public readonly IUsersManagerService _usersManagerService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly WebSocketHandler _webSocketHandler;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context, ILogger<UserController> logger, IUsersManagerService usersManagerService , WebSocketHandler webSocketHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _usersManagerService = usersManagerService;
            _webSocketHandler = webSocketHandler;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var re = await _usersManagerService.GetUsersListAsync(page, pageSize);
            if (re.IsSuccess)
            {
                var data = re.Data as dynamic;
                
                ViewBag.TotalUsers = data?.totalUsers;
                ViewBag.TotalPage = data?.totalPages;
                ViewBag.currentPageSize = data?.currentPageSize;
                ViewBag.CurrentPage = data?.currentPage;
                var users = data?.users as List<ViewUser>;
                return View(users);
                
            }
            return NotFound();
        }
        [HttpGet]
        [Route("{id}/View")]
        public async Task<IActionResult> ViewUser(string id)
        {
            ViewUser UserInfo = await _usersManagerService.GetUserByIdAsync(id);
            if (UserInfo == null)
            {
                return NotFound();
            }
            return View(UserInfo);
        }
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUser model)
        {
            if (ModelState.IsNullOrEmpty())
            {
                return View();
            }
            var Result = await _usersManagerService.CreateUserAsync(model);
            if (!Result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, Result.Message);
                return View(model);
            }
            TempData["UsersMessage"] = "Thêm người dùng thành công";
            TempData["Type"] = "success";
            await _webSocketHandler.SendMessageAsync($"Data has been updated", "/admin/user");
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("{id}/Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            ViewUser UserInfo = await _usersManagerService.GetUserByIdAsync(id);
            if (UserInfo == null)
            {
                return NotFound();
            }
            return View((UserInfo, new EditUser()));
        }
        [HttpPost]
        [Route("{id}/Edit")]
        public async Task<IActionResult> UpdateUser([Bind(Prefix = "Item2")] EditUser userModel, string id)
        {
            if (ModelState.IsNullOrEmpty())
            {
                TempData["UsersMessage"] = "Không có dữ liệu nào được gửi";
                TempData["Type"] = "error";
                return RedirectToAction("Index", "User");
            }
            var Result = await _usersManagerService.UpdateUserAsync(id, userModel);
            if (!Result.IsSuccess)
            {
                TempData["UsersMessage"] = Result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "User");
            }
            TempData["UsersMessage"] = Result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("ViewUser", "User", new { id = id });
        }
        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var Result = await _usersManagerService.DeleteUserAsync(id);
            if (!Result.IsSuccess)
            {
                TempData["UsersMessage"] = Result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "User");
            }
            TempData["UsersMessage"] = Result.Message;
            TempData["Type"] = "success";
            await _webSocketHandler.SendMessageAsync($"Data has been updated", "/admin/user");
            return RedirectToAction("Index", "User");
        }
    }
}
