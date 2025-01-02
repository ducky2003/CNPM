using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models.Book
{
    [BindProperties]
    public class UpdateBook
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? Language { get; set; }
        public string? Pages { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
        public bool? IsEBook { get; set; }
        public IFormFile? Pdf { get; set; }
        public bool IsPublish { get; set; }
    }
}
