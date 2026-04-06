using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class BookingSystemDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new BookingDbContext(optionsBuilder.Options);
        }
    }
}
