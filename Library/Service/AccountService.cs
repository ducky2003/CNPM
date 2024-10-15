using Microsoft.AspNetCore.Mvc;
using Library.Data;
using Library.Entities;
using Library.Models;
using Microsoft.AspNetCore.Identity;
namespace Library.Service
{
    public class AuthStatus
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string>? Role { get; set; }
    }
    public interface IAccountService
    {
        Task<AuthStatus> LoginAsync(LoginModel u);
        Task<AuthStatus> LogoutAsync();
    }
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(SignInManager<User> signInManager, UserManager<User> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AccountService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthStatus> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if (user == null)
                {
                    return new AuthStatus
                    {
                        IsSuccess = false,
                        ErrorMessage = "Thông tin đăng nhập không chính xác"
                    };
                }
                var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    int accessFailedRemaining = 5 - accessFailedCount;
                    if (result.IsLockedOut)
                    {
                        var getLockedTimeUntil = await _userManager.GetLockoutEndDateAsync(user);
                        var getLockedTime = getLockedTimeUntil - DateTime.Now;
                        if (getLockedTime.Value.TotalMinutes < 6)
                        {
                            return new AuthStatus
                            {
                                IsSuccess = false,
                                ErrorMessage = $"Tài khoản bị khóa do đăng nhập sai nhiều lần, vui lòng thử lại sau: {getLockedTime.Value.ToString("mm")} phút"
                            };
                        }
                        return new AuthStatus
                        {
                            IsSuccess = false,
                            ErrorMessage = "Tài khoản của bạn đã bị khóa bởi quản trị viên"
                        };
                    }
                    if (accessFailedCount < 2)
                    {
                        return new AuthStatus
                        {
                            IsSuccess = false,
                            ErrorMessage = "Thông tin đăng nhập không chính xác"
                        };
                    }
                    return new AuthStatus
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Thông tin đăng nhập không chính xác, bạn còn {accessFailedRemaining} lần thử"
                    };
                }
                await _userManager.ResetAccessFailedCountAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                return new AuthStatus
                {
                    IsSuccess = true,
                    Role = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new AuthStatus
                {
                    IsSuccess = false,
                    ErrorMessage = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<AuthStatus> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new AuthStatus
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new AuthStatus
                {
                    IsSuccess = false,
                    ErrorMessage = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
    }
}
