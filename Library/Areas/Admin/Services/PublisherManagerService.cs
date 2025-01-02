using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Library.Areas.Admin.Models;
using Library.Data;
using Library.Entities;
using Library.Models;
using Library.Utils;
using Slugify;
namespace Library.Areas.Admin.Services
{
    public interface IPublisherService
    {
        Task<ActionResponse> GetPublishers(int? page, int? pageSize);
        Task<ActionResponse> GetListPublisherAsync();
        Task<ActionResponse> CreatePublisherAsync(CreatePublisher newPublisher);
        Task<ActionResponse> UpdatePublisherAsync(UpdatePublisher updatePublisher);
        Task<ActionResponse> DeletePublisherAsync(int publisherId);
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
                    Message = "Lỗi khi lấy danh sach"
                };
            }
        }
        public async Task<ActionResponse> GetListPublisherAsync()
        {
            try
            {
                var publishers = await _context.Publisher.Select(publisher => new PublisherView()
                {
                    PublisherId = publisher.PublisherId,
                    Name = publisher.Name,
                }).ToListAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nhà xuất bản thành công",
                    Data = publishers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh sách nhà xuất bản"
                };
            }
        }
        public async Task<ActionResponse> CreatePublisherAsync(CreatePublisher newPublisher)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return new ActionResponse()
                    {
                        IsSuccess = false,
                        Message = "Bạn cần đăng nhập để thực hiện chức năng này"
                    };
                }
                var publisher = new Publisher()
                {
                    Name = newPublisher.Name,
                    Slug = new SlugHelper().GenerateSlug(newPublisher.Name),
                    Address = newPublisher.GetAddress(),
                    AddedAt = DateTime.UtcNow,
                    AddedById = userId
                };
                await _context.Publisher.AddAsync(publisher);
                await _context.SaveChangesAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Thêm nhà xuất bản thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Không thể thêm nhà xuất bản"
                };
            }
        }
        public async Task<ActionResponse> UpdatePublisherAsync(UpdatePublisher updatePublisher)
        {
            try
            {
                var publisher = await _context.Publisher.FindAsync(updatePublisher.PublisherId);
                if (publisher == null)
                {
                    return new ActionResponse()
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nhà xuất bản"
                    };
                }
                publisher.Name = updatePublisher.Name ?? publisher.Name;
                publisher.Slug = publisher.Name != null ? new SlugHelper().GenerateSlug(publisher.Name) : publisher.Slug;
                publisher.Address = updatePublisher.Address ?? publisher.Address;
                await _context.SaveChangesAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Cập nhật nhà xuất bản thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Không thể cập nhật nhà xuất bản"
                };
            }
        }
        public async Task<ActionResponse> DeletePublisherAsync(int publisherId)
        {
            try
            {
                var publisher = await _context.Publisher.FindAsync(publisherId);
                if (publisher == null)
                {
                    return new ActionResponse()
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nhà xuất bản"
                    };
                }
                _context.Publisher.Remove(publisher);
                await _context.SaveChangesAsync();
                return new ActionResponse()
                {
                    IsSuccess = true,
                    Message = "Xóa nhà xuất bản thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse()
                {
                    IsSuccess = false,
                    Message = "Không thể xóa nhà xuất bản"
                };
            }
        }
    }
}
