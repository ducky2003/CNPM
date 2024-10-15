using Library.Data;
using Library.Models;
using Library.Utils;
using Library.Areas.Admin.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Areas.Admin.Services
{
    public interface IAuthorManagerService
    {
        Task<ActionResponse> GetAuthor(int? page, int? pageSize);
        Task<ActionResponse> GetListAuthor();
    }
    public class AuthorManagerService : IAuthorManagerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthorManagerService> _logger;
        public AuthorManagerService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AuthorManagerService> logger)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<ActionResponse> GetAuthor(int? page, int? pageSize)
        {
            try
            {
                var query = _context.Author.AsQueryable();
                var totalAuthors = await query.CountAsync();
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling((double)totalAuthors / currentPageSize);
                var result = await query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToListAsync();
                var author = result
                .Select(author => new AuthorView()
                {
                    AuthorId = author.AuthorId,
                    Name = author.Name ?? "Chưa có tên",
                    Address = author.Address ?? "Chưa có địa chỉ",
                    AuthorImg = author.AuthorImg ?? "Chưa có ảnh",
                    Title = author.Title ?? "Chưa có",
                    AddedAt = author.AddedAt, 
                    AddByName = author.AddBy != null ?author.AddBy.FirstName + " " + author.AddBy.LastName : "N/A"
                }).ToList();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tác giả thành công",
                    Data = new
                    {
                        totalAuthors,
                        totalPages,
                        currentPage,
                        currentPageSize,
                        author
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorManagerService.GetAuthor at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể lấy danh sách tác giả, vui lòng thử lại sau"
                };
            }
        }
        public async Task<ActionResponse> GetListAuthor()
        {
            try
            {
                var authors = await _context.Author.Select(author => new AuthorView()
                {
                    AuthorId = author.AuthorId,
                    Name = author.Name,
                }).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tác giả thành công",
                    Data = authors
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorManagerService.GetListAuthor at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể lấy danh sách tác giả, vui lòng thử lại sau"
                };
            }
        }
    }
}
