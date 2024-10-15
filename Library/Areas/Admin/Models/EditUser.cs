using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class EditUser
    {
        public string? Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public IFormFile? ImageFile { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}
