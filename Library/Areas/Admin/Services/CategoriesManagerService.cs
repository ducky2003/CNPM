using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Slugify;
using Library.Areas.Admin.Models;
using Library.Data;
using Library.Entities;
using Library.Models;
using Library.Utils;

namespace Library.Areas.Admin.Services
{
    public interface ICategoriesManagerService
    {
        Task<ActionResponse> GetCategoriesAsync(int? page, int? pageSize);
        Task<ActionResponse> GetListCategoryAsync();
        Task<ActionResponse> CreateCategoryAsync(CreateCategory newCategory);
        Task<ActionResponse> UpdateCategoryAsync(int categoryId, string name);
        Task<ActionResponse> HideCategoryAsync(int categoryId);
        Task<ActionResponse> ShowCategoryAsync(int categoryId);
        Task<ActionResponse> DeleteCategoryAsync(int categoryId);
    }
    public class CategoriesManagerService : ICategoriesManagerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoriesManagerService> _logger;
        public CategoriesManagerService(ApplicationDbContext context, ILogger<CategoriesManagerService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<ActionResponse> GetCategoriesAsync(int? page, int? pageSize)
        {
            try
            {
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var query = _context.Category.AsQueryable();
                var totalCategories = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCategories / currentPageSize);
                var categories = await query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .Select(category => new CategoryViewModel()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    CreatedAt = category.CreatedAt,
                    CreatedByName = category.CreatedBy.FirstName + " " + category.CreatedBy.LastName,
                    Status = category.Status
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh mục thành công",
                    Data = new
                    {
                        totalCategories,
                        totalPages,
                        currentPage,
                        currentPageSize,
                        categories
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Không thể lấy danh mục"
                };
            }
        }
        public async Task<ActionResponse> GetListCategoryAsync()
        {
            try
            {
                var categories = await _context.Category.Select(category => new CategoryViewModel()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách danh mục thành công",
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Không thể lấy danh sách danh mục"
                };
            }
        }
        public async Task<ActionResponse> CreateCategoryAsync(CreateCategory category)
        {
            var result = new ActionResponse();
            try
            {
                string UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(UserId))
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy người dùng để thêm danh mục";
                    return result;
                }
                var slugify = new SlugHelper();
                var newCategory = new Entities.Category()
                {
                    Name = category.Name,
                    Slug = slugify.GenerateSlug(category.Name),
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = UserId,
                    Status = category.Status
                };
                await _context.Category.AddAsync(newCategory);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Thêm danh mục thành công";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.IsSuccess = false;
                result.Message = "Không thể thêm danh mục";
            }
            return result;
        }
        public async Task<ActionResponse> HideCategoryAsync(int categoryId)
        {
            var result = new ActionResponse();
            try
            {
                var category = await _context.Category.FindAsync(categoryId);
                if (category == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy danh mục";
                    return result;
                }
                category.Status = false;
                _context.Category.Update(category);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.Message = "Ẩn danh mục thành công";
            }
            catch (System.Exception)
            {
                result.IsSuccess = false;
                result.Message = "Không thể ẩn danh mục";
            }
            return result;
        }
        public async Task<ActionResponse> ShowCategoryAsync(int categoryId)
        {
            var result = new ActionResponse();
            try
            {
                var category = await _context.Category.FindAsync(categoryId);
                if (category == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy danh mục";
                    return result;
                }
                category.Status = true;
                _context.Category.Update(category);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.Message = "Đã hiển thị danh mục thành công";
            }
            catch (System.Exception)
            {
                result.IsSuccess = false;
                result.Message = "Không thể hiện danh mục";
            }
            return result;
        }
        public async Task<ActionResponse> DeleteCategoryAsync(int categoryId)
        {
            var result = new ActionResponse();
            try
            {
                var category = await _context.Category.FindAsync(categoryId);
                if (category == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy danh mục";
                    return result;
                }
                _context.Category.Remove(category);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.Message = "Xóa danh mục thành công";
            }
            catch (System.Exception)
            {
                result.IsSuccess = false;
                result.Message = "Không thể xóa danh mục";
            }
            return result;
        }
        public async Task<ActionResponse> UpdateCategoryAsync(int categoryId, string name)
        {
            var result = new ActionResponse();
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    result.IsSuccess = false;
                    result.Message = "Tên danh mục không hợp lệ";
                    return result;
                }
                var category = await _context.Category.FindAsync(categoryId);
                if (category == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy danh mục";
                    return result;
                }
                category.Name = name;
                _context.Category.Update(category);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.Message = "Cập nhật danh mục thành công";
            }
            catch (System.Exception)
            {
                result.IsSuccess = false;
                result.Message = "Không thể cập nhật danh mục";
            }
            return result;
        }
    }

}