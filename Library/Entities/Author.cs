using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Library.Entities
{
    [Table("Author")]
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string Name { get; set; } = null!;
        public string? AuthorImg { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Title { get; set; }
        public DateTime AddedAt { get; set; }
        [ForeignKey("User")]
        public string AddById { get; set; } = null!;
        public virtual User AddBy { get; set; } = null!;
    }
}
