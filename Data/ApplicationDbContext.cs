using Microsoft.EntityFrameworkCore;
using SmartPOS.Models;

namespace SmartPOS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // এই দুটি লাইন ডাটাবেজে টেবিল তৈরি করতে সাহায্য করবে
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}