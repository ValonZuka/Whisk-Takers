using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WhiskTakers.Infrastructure
{
    public class WhiskTakersDbContextFactory : IDesignTimeDbContextFactory<WhiskTakersDbContext>
    {
        public WhiskTakersDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read connection string
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WhiskTakersDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new WhiskTakersDbContext(optionsBuilder.Options);
        }
    }
}
