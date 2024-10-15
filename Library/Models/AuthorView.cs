namespace Library.Models
{
    public class AuthorView
    {
        public int AuthorId { get; set; }
        public string? Name { get; set; }
        public string? AuthorImg { get; set; }
        public string? Address { get; set; }
        public string? Title { get; set; }
        public DateTime? AddedAt { get; set; }
        public string? AddByName { get; set; }
    }
}
