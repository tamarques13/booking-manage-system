using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class BookingDbContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Resource)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
            .HasOne(u => u.User)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.ResourceId, r.UserId , r.StartDate, r.EndDate, r.Status});
        }

    }
}