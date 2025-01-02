using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Name { get; set; } = null!;
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsEBook { get; set; }
        public string? PdfUrl { get; set; }
        public string? Language { get; set; }
        public string? Pages { get; set; }
        public string? Price { get; set; }
        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("AuthorId")]
        public int? AuthorId { get; set; }
        public virtual Author Author { get; set; } = null!;
        [ForeignKey("PublisherId")]
        public int? PublisherId { get; set; }
        public virtual Publisher Publisher { get; set; } = null!;
        public int? ReaderCount { get; set; }
        public int? Rating { get; set; }
        [ForeignKey("User")]
        public string AddedById { get; set; } = null!;
        public virtual User AddedBy { get; set; } = null!;
        public DateTime AddedAt { get; set; }
        [ForeignKey("User")]
        public string? UpdateById { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublish { get; set; }
    }
}
