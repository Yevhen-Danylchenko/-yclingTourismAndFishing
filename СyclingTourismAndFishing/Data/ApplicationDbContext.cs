using Microsoft.EntityFrameworkCore;
using CyclingTourismAndFishing.Models;

namespace CyclingTourismAndFishing.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфігурація зв’язку User ↔ CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId);

            // Конфігурація зв’язку Item ↔ CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Item)
                .WithMany()
                .HasForeignKey(c => c.ItemId);

            // Конфігурація для decimal Price
            modelBuilder.Entity<Item>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
