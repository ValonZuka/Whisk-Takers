using Microsoft.EntityFrameworkCore;
using WhiskTakers.Core.Entities;

namespace WhiskTakers.Infrastructure
{
    public class WhiskTakersDbContext : DbContext
    {
        public WhiskTakersDbContext(DbContextOptions<WhiskTakersDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Fabrika> Fabrika { get; set; }
        public DbSet<Puntori> Puntori { get; set; }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 

                
            modelBuilder.Entity<Fabrika>()
                .HasMany(f => f.Puntori)
                .WithOne(p => p.Fabrika)
                .HasForeignKey(p => p.FabrikaId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Puntori>()
                .HasOne(p => p.Fabrika)
                .WithMany(f => f.Puntori)
                .HasForeignKey(p => p.FabrikaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
