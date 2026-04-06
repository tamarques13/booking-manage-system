using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BookingSystem.Infrastructure.Persistence.Configurations;
using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Reservations
{
    public class AdminReservationRepository(BookingDbContext context) : IAdminReservationRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task<List<Reservation>> GetAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[]? status, Guid? userId)

        {
            IQueryable<Reservation> query = _context.Reservations;

            if (userId != null) query = query.Where(r => r.UserId == userId);

            if (resourceId.HasValue) query = query.Where(x => x.ResourceId == resourceId.Value);

            if (endTime.HasValue && startTime.HasValue) query = query.Where(x => x.StartDate <= endTime.Value && x.EndDate >= startTime.Value);

            if (status != null && status.Length != 0) query = query.Where(x => status.Contains(x.Status));

            return await query.ToListAsync();
        }

        public async Task<Reservation> GetByIdAsync(Guid reservationId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId) ?? throw new KeyNotFoundException($"Reservation with Id {reservationId} not found.");
        }
    }
}