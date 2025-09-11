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

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

    }
}
