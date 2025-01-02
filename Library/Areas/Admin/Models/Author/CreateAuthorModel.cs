using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class CreateAuthorModel
    {
        public IFormFile? Image { get; set; }
        public string Name { get; set; } = null!;
        public string? Province { get; set; }
        public string? District { get; set; }
        public string Title { get; set; } = null!;
        public string GetAddress()
        {
            return $"{District}, {Province}";
        }
    }
}