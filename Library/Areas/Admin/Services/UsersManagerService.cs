using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Library.Utils;
using Library.Entities;
using Library.Data;
using Library.Areas.Admin.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
namespace Library.Areas.Admin.Services
{
    public interface IUsersManagerService
    {
        Task<ActionResponse> GetUsersListAsync(int? page, int? pageSize);
        Task<ViewUser> GetUserByIdAsync(string userId);
        Task<ActionResponse> DeleteUserAsync(string userId);
        Task<ActionResponse> CreateUserAsync(CreateUser user);
        Task<ActionResponse> UpdateUserAsync(string id, EditUser user);
    }
    public class UsersManagerService : IUsersManagerService
    {
        private readonly ILogger<UsersManagerService> _logger;
        public readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersManagerService(ILogger<UsersManagerService> logger, UserManager<User> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ActionResponse> GetUsersListAsync(int? page, int? pageSize)
        {
            try
            {
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var query = _userManager.Users.AsQueryable();
                var totalUsers = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalUsers / currentPageSize);
                var result = await query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToListAsync();
                var users = result.Select(user => new ViewUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImage = user.ProfileImage,
                    
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    CreatedAt = user.CreatedAt,
                    IsLocked = _userManager.IsLockedOutAsync(user).Result
                }).ToList();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách người dùng thành công",
                    Data = new
                    {
                        users,
                        totalPages,
                        currentPage,
                        currentPageSize,
                        totalUsers
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể lấy danh sách người dùng"
                };
            }

        }
        public async Task<ViewUser> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    throw new Exception("Người dùng không tồn tại");
                var UserInfo = new ViewUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImage = user.ProfileImage,
                    //Address = user.Address,
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    CreatedAt = user.CreatedAt,
                    IsLocked = _userManager.IsLockedOutAsync(user).Result
                };
                return UserInfo;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }
        public async Task<ActionResponse> UpdateUserAsync(string id, EditUser user)
        {
            try
            {
                var CurrentUser = await _userManager.FindByIdAsync(id);
                if (CurrentUser is null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Người dùng không tồn tại"
                    };
                }
                CurrentUser.Email = user.Email ?? CurrentUser.Email;
                CurrentUser.FirstName = user.FirstName ?? CurrentUser.FirstName;
                CurrentUser.LastName = user.LastName ?? CurrentUser.LastName;
                
                CurrentUser.PhoneNumber = user.Phone ?? CurrentUser.PhoneNumber;
                CurrentUser.DateOfBirth = user.DateOfBirth ?? CurrentUser.DateOfBirth;
                
                if (user.ImageFile != null)
                {
                    CurrentUser.ProfileImage = UploadImage.UploadSingleImage(user.ImageFile) ?? CurrentUser.ProfileImage;
                }
                var CurrentRole = await _userManager.GetRolesAsync(CurrentUser);
                if (!CurrentRole.Contains(user.RoleName) && user.RoleName != null)
                {
                    var RemoveRole = await _userManager.RemoveFromRolesAsync(CurrentUser, CurrentRole);
                    var AddRole = await _userManager.AddToRoleAsync(CurrentUser, user.RoleName);
                    if (!AddRole.Succeeded)
                    {
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Không thể cập nhật thông tin người dùng"
                        };
                    }
                }
                var Result = await _userManager.UpdateAsync(CurrentUser);
                if (!Result.Succeeded)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không thể cập nhật thông tin người dùng"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật thông tin người dùng thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể cập nhật thông tin người dùng"
                };
            }
        }
        public async Task<ActionResponse> DeleteUserAsync(string userId)
        {
            if (userId == _userManager.GetUserId(_httpContextAccessor?.HttpContext?.User))
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể xoá tài khoản của chính mình"
                };
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại"
                };
            }
            var Result = await _userManager.DeleteAsync(user);
            if (!Result.Succeeded)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể xoá người dùng này"
                };
            }
            return new ActionResponse
            {
                IsSuccess = true,
                Message = "Đã xoá người dùng thành công"
            };
        }
        public async Task<ActionResponse> CreateUserAsync(CreateUser user)
        {
            var IsEmailUsed = await _userManager.FindByEmailAsync(user.Email);
            if (IsEmailUsed != null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Email đã được sử dụng"
                };
            }
            var NewUser = new User
            {
                Email = user.Email,
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                //Address = user.GetAddress(),
                //DateOfBirth = user.DateOfBirth,
                ProfileImage = UploadImage.UploadSingleImage(user.ProfileImage) ?? "/img/default-user.webp",
                CreatedAt = DateTime.UtcNow,
                //WorkspaceId = user.WorkspaceId,
            };
            var CreateNewUser = await _userManager.CreateAsync(NewUser, user.GeneratePassword());
            if (!CreateNewUser.Succeeded)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể tạo người dùng, vui lòng thử lại sau!"
                };
            }
            var AddRole = await _userManager.AddToRoleAsync(NewUser, user.RoleName);
            if (!AddRole.Succeeded)
            {
                await _userManager.DeleteAsync(NewUser);
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể tạo người dùng, lỗi phân quyền!"
                };
            }
            return new ActionResponse
            {
                IsSuccess = true,
                Message = "Tạo người dùng thành công!"
            };
        }
    }
}
