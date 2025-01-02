using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class UpdateAuthorModel
    {
        public int AuthorId { get; set; }
        public IFormFile? Image { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}