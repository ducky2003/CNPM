using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Library.Entities
{
    [Table("Publisher")]
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public DateTime AddedAt { get; set; }
        [Required]
        [ForeignKey("User")]
        public string AddedById { get; set; } = null!;
        public virtual User AddedBy { get; set; } = null!;
    }
}
