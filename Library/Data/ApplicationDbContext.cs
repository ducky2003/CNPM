using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Library.Entities;
using Microsoft.CodeAnalysis.Options;
using Library.Models;
namespace Library.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Author> Author { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
    }
}