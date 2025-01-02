namespace Library.Models
{
    public class ViewBook
    {
        public int BookId { get; set; }
        public string? Slug { get; set; }
        public string? Name { get; set; }
        public string? ImageURL { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorAddress { get; set; }
        public string? AuthorImageURL { get; set; }
        public int? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Pages { get; set; }
        public string? Language { get; set; }
        public int ReaderCount { get; set; }
        public DateTime? AddedAt { get; set; }
        public string? AddedById { get; set; }
        public string? AddedByName { get; set; }
        public bool? IsPublish { get; set; }
    }
}
