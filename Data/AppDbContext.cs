using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;

namespace BookingSystem.Data
{
    public class BookingDbContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {

        }
    }
}