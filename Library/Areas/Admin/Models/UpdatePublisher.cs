using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class UpdatePublisher
    {
        public int PublisherId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
