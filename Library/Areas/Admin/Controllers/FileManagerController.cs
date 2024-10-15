using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Library.Config;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/file-manager")]
    [Authorize(Roles = AppRole.Admin)]
    public class FileManagerController : Controller
    {
        IWebHostEnvironment _env;
        public FileManagerController(IWebHostEnvironment env) => _env = env;
        public IActionResult Index()
        {
            return View();
        }
        [Route("Connector")]
        public async Task<IActionResult> Connector()
        {
            var connector = GetConnector();
            return await connector.ProcessAsync(Request);
        }
        [HttpGet]
        [Route("/Thumb/{hash}")]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }
        private Connector GetConnector()
        {
            // Thư mục gốc lưu trữ là wwwwroot/files (đảm bảo có tạo thư mục này)
            string pathRoot = "Uploads";
            var driver = new FileSystemDriver();
            string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
            string requestUrl = "uploads";
            var uri = new Uri(absoluteUrl);
            // .. ... wwww/files
            string rootDirectory = Path.Combine(_env.ContentRootPath, pathRoot);
            // https://localhost:5001/files/
            string url = $"{uri.Scheme}://{uri.Authority}/{requestUrl}/";
            string urlThumb = $"{uri.Scheme}://{uri.Authority}/Admin/file-manager/Thumb/";
            var root = new RootVolume(rootDirectory, url, urlThumb)
            {
                IsReadOnly = !User.IsInRole(AppRole.Admin),
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "/", // Beautiful name given to the root/home folder
                //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
                //LockedFolders = new List<string>(new string[] { "Folder1" }
                ThumbnailSize = 500,
            };
            driver.AddRoot(root);
            return new Connector(driver)
            {
                // This allows support for the "onlyMimes" option on the client.
                MimeDetect = MimeDetectOption.Internal
            };
        }
    }
}