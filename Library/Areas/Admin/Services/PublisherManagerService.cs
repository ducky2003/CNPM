using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Library.Areas.Admin.Models;
using Library.Data;
using Library.Entities;
using Library.Models;
using Library.Utils;
namespace Library.Areas.Admin.Services
{
    public interface IPublisherService
    {
        Task<ActionResponse> GetPublishers(int? page, int? pageSize);
    }
    public class PublisherManagerService : IPublisherService
    {
        public readonly ApplicationDbContext _context;
        public readonly ILogger<PublisherManagerService> _logger;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public PublisherManagerService(ApplicationDbContext context, ILogger<PublisherManagerService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ActionResponse> GetPublishers(int? page, int? pageSize)
        {
            try
            {
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var query = _context.Publisher.AsQueryable();
                var totalPublishers = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalPublishers / currentPageSize);
                var publishers = await query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .Select(publisher => new PublisherView()
                {
                    PublisherId = publisher.PublisherId,
                    Name = publisher.Name,
                    AddedAt = publisher.AddedAt,
                    AddedByName = publisher.AddedBy.FirstName + " " + publisher.AddedBy.LastName,
                    Address = publisher.Address
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nhà xuẩt bản thành công",
                    Data = new
                    {
                        totalPublishers,
                        totalPages,
                        currentPage,
                        currentPageSize,
                        publishers
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh mục"
                };
            }
        }
    }
}
