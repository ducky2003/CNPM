using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Models
{
    [BindProperties]
    public class CreatePublisher
    {
        public string Name { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string GetAddress()
        {
            return $"{District}, {Province}";
        }
    }
}
