using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace Library.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        [BindProperty]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]

        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [StringLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự.")]
        [BindProperty]
        public string Password { get; set; } = null!;

        [BindProperty]
        public bool RememberMe { get; set; }

       
        
    }
}
