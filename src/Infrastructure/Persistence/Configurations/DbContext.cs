using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
    {
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ResourceId, r.UserId, r.StartDate, r.EndDate, r.Status });

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => new { r.Token, r.UserId });
        }

    }
}