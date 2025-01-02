namespace Library.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public int? TotalBooks { get; set; }
        public bool? Status { get; set; } = true;
    }
}