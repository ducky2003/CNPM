using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
namespace Library.Entities
{
    public class User : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; } = null;
        [Required]
        public string? LastName { get; set; } = null;
        public string? ProfileImage { get; set; } = null;
        public DateTime DateOfBirth { get; set; }
        //public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
