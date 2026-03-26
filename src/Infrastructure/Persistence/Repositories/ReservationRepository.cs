using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BookingSystem.Infrastructure.Persistence.Configurations;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories
{
    public class ReservationRepository(BookingDbContext context) : IReservationRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);

            await _context.SaveChangesAsync();
        }

        public async Task<Reservation> GetByIdAsync(Guid reservationId, Guid userId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId) ?? throw new KeyNotFoundException($"Reservation with Id {reservationId} not found.");
        }

        public async Task<List<Reservation>> GetAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[] status, Guid userId)

        {
            IQueryable<Reservation> query = _context.Reservations.Where(r => r.UserId == userId);

            if (resourceId.HasValue) query = query.Where(x => x.ResourceId == resourceId.Value);

            if (endTime.HasValue && startTime.HasValue) query = query.Where(x => x.StartDate <= endTime.Value && x.EndDate >= startTime.Value);

            if (status.Length != 0) query = query.Where(x => status.Contains(x.Status));

            return await query.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Reservation, bool>> predicate)
        {
            return await _context.Reservations.AnyAsync(predicate);
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

        }

        public async Task<List<Reservation>> GetAdminAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[]? status, Guid? userId)

        {
            IQueryable<Reservation> query = _context.Reservations;
            
            if (userId != null) query = query.Where(r => r.UserId == userId);

            if (resourceId.HasValue) query = query.Where(x => x.ResourceId == resourceId.Value);

            if (endTime.HasValue && startTime.HasValue) query = query.Where(x => x.StartDate <= endTime.Value && x.EndDate >= startTime.Value);

            if (status != null && status.Length != 0) query = query.Where(x => status.Contains(x.Status));

            return await query.ToListAsync();
        }

        public async Task<Reservation> GetAdminByIdAsync(Guid reservationId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId) ?? throw new KeyNotFoundException($"Reservation with Id {reservationId} not found.");
        }
    }
}