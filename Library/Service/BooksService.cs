using Library.Data;
using Library.Models;
using Library.Utils;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public interface IBooksService
    {
        Task<ActionResponse> GetCategoriesAsync();
        Task<ActionResponse> GetAuthorsAsync();
        Task<ActionResponse> GetListBookAsync(int? page, int? pageSize, string? categoryName = null, string? authorName = null);
        Task<ActionResponse> GetBookDetailAsync(int id);
    }
    public class BooksService : IBooksService
    {
        public readonly ApplicationDbContext _context;
        public readonly ILogger<BooksService> _logger;
        public BooksService(ApplicationDbContext context, ILogger<BooksService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ActionResponse> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Category.AsQueryable()
                .Where(category => category.Status == true)
                .Take(10)
                .Select(category => new CategoryViewModel()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    TotalBooks = category.Books.Count(),
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh mục thành công",
                    Data = categories
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
        public async Task<ActionResponse> GetAuthorsAsync()
        {
            try
            {
                var authors = await _context.Author.AsQueryable()
                .Take(10)
                .Select(author => new AuthorView()
                {
                    AuthorId = author.AuthorId,
                    Name = author.Name,
                    TotalBooks = author.Books.Count(),
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh mục thành công",
                    Data = authors
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
        public async Task<ActionResponse> GetListBookAsync(int? page, int? pageSize, string? categoryName = null, string? authorName = null)
        {
            try
            {
                var query = _context.Books.AsQueryable();
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    query = query.Where(book => book.Category.Name == categoryName);
                }
                if (!string.IsNullOrEmpty(authorName))
                {
                    query = query.Where(book => book.Author.Name == authorName);
                }
                var totalBooks = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalBooks / currentPageSize);
                var books = await query
                    .Where(book => book.IsPublish == true)
                    .Where(book => book.Category.Status == true)
                    .OrderByDescending(book => book.ReaderCount)
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .Select(book => new ViewBook
                    {
                        BookId = book.BookId,
                        Slug = book.Slug,
                        Name = book.Name,
                        ImageURL = book.ImageUrl,
                        AuthorId = book.AuthorId,
                        AuthorName = book.Author.Name,
                        AuthorImageURL = book.Author.AuthorImg,
                        Pages = book.Pages,
                        ReaderCount = book.ReaderCount ?? 0,
                    }).ToListAsync();
                totalBooks = books.Count;
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách sách thành công",
                    Data = new
                    {
                        totalBooks,
                        totalPages,
                        currentPage,
                        currentPageSize,
                        books
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lấy danh sách sách thất bại"
                };
            }
        }
        public async Task<ActionResponse> GetBookDetailAsync(int id)
        {
            try
            {
                var book = await _context.Books.AsQueryable()
                    .Where(book => book.BookId == id)
                    .Where(book => book.IsPublish == true)
                    .Where(book => book.Category.Status == true)
                    .Select(book => new ViewBook
                    {
                        BookId = book.BookId,
                        Slug = book.Slug,
                        Name = book.Name,
                        ImageURL = book.ImageUrl,
                        AuthorId = book.AuthorId,
                        AuthorName = book.Author.Name,
                        AuthorAddress = book.Author.Address,
                        AuthorImageURL = book.Author.AuthorImg,
                        Pages = book.Pages,
                        ReaderCount = book.ReaderCount ?? 0,
                        CategoryId = book.CategoryId,
                        CategoryName = book.Category.Name,
                        ShortDescription = book.ShortDescription,
                        Description = book.Description,
                        IsPublish = book.IsPublish,
                        AddedAt = book.AddedAt,
                    }).FirstOrDefaultAsync();
                if (book == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sách"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin sách thành công",
                    Data = book
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lấy thông tin sách thất bại"
                };
            }

        }
    }
}