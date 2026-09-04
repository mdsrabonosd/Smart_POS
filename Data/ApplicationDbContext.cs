using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Models;

namespace SmartPOS.Data
{
    // DbContext এর বদলে IdentityDbContext ব্যবহার করা হয়েছে রোল এবং ইউজার টেবিল ম্যানেজ করার জন্য
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // এখানে আপনার পূর্বের কোনো কাস্টম রিলেশনশিপ কনফিগারেশন থাকলে তা থাকবে
        }
    }
}