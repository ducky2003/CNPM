using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class CreateCategory
    {
        public string? Name { get; set; }
        public bool Status { get; set; }
    }
}
