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
    }
}
