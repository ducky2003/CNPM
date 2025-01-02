using Library.Data;
using Library.Models;
using Library.Utils;
using Slugify;
using Library.Areas.Admin.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Library.Entities;

namespace Library.Areas.Admin.Services
{
    public interface IAuthorManagerService
    {
        Task<ActionResponse> GetAuthor(int? page, int? pageSize);
        Task<ActionResponse> GetListAuthor();
        Task<ActionResponse> CreateAuthorAsync(CreateAuthorModel author);
        Task<ActionResponse> UpdateAuthorAsync(UpdateAuthorModel author);
        Task<ActionResponse> DeleteAuthorAsync(int authorId);
        Task<ActionResponse> SearchAuthorsAsync(string? searchQuery);
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
        public async Task<ActionResponse> CreateAuthorAsync(CreateAuthorModel author)
        {
            try
            {
                string userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Người dùng không hợp lệ"
                    };
                }
                var slugHelper = new SlugHelper();
                var authorImage = UploadImage.UploadSingleImage(author.Image) ?? "/img/default-user.webp";
                var newAuthor = new Author
                {
                    Name = author.Name,
                    Slug = slugHelper.GenerateSlug(author.Name),
                    Address = author.GetAddress(),
                    AuthorImg = authorImage,
                    Title = author.Title,
                    AddedAt = DateTime.UtcNow,
                    AddById = userId
                };
                await _context.Author.AddAsync(newAuthor);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Thêm tác giả thành công"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorsManagerService.CreateAuthorAsync at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể thêm tác giả, vui lòng thử lại sau"
                };
            }
        }
        public async Task<ActionResponse> UpdateAuthorAsync(UpdateAuthorModel author)
        {
            try
            {
                var slugHelper = new SlugHelper();
                var authorToUpdate = await _context.Author.FindAsync(author.AuthorId);
                if (authorToUpdate == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tác giả"
                    };
                }
                authorToUpdate.Name = author.Name ?? authorToUpdate.Name;
                authorToUpdate.Slug = slugHelper.GenerateSlug(author.Name) ?? authorToUpdate.Slug;
                authorToUpdate.Address = author.Address ?? authorToUpdate.Address;
                authorToUpdate.AuthorImg = author.Image != null ? UploadImage.UploadSingleImage(author.Image) : authorToUpdate.AuthorImg;
                authorToUpdate.Title = author.Title ?? authorToUpdate.Title;
                _context.Author.Update(authorToUpdate);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật tác giả thành công"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorsManagerService.UpdateAuthorAsync at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể cập nhật tác giả, vui lòng thử lại sau"
                };
            }
        }
        public async Task<ActionResponse> DeleteAuthorAsync(int authorId)
        {
            try
            {
                var authorToDelete = await _context.Author.FindAsync(authorId);
                if (authorToDelete == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tác giả"
                    };
                }
                _context.Author.Remove(authorToDelete);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Xóa tác giả thành công"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorsManagerService.DeleteAuthorAsync at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể xóa tác giả, vui lòng thử lại sau"
                };
            }
        }
        public async Task<ActionResponse> SearchAuthorsAsync(string? searchQuery)
        {
            try
            {
                var query = _context.Author.AsQueryable();
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(author => author.Name.Contains(searchQuery)).OrderByDescending(author => author.AddedAt);
                }
                query = query.Take(10);
                var authors = await query.Select(author => new AuthorView()
                {
                    AuthorId = author.AuthorId,
                    Name = author.Name,
                    Address = author.Address,
                    AuthorImg = author.AuthorImg,
                    Slug = author.Slug,
                    Title = author.Title,
                    AddedAt = author.AddedAt,
                }).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Tìm kiếm tác giả thành công",
                    Data = new
                    {
                        authors
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, $"AuthorsManagerService.SearchAuthorsAsync at: {DateTime.UtcNow}");
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể tìm kiếm tác giả, vui lòng thử lại sau"
                };
            }
        }
    }
}
