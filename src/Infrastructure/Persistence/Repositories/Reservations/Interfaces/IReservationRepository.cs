using BookingSystem.Domain.Models;
using System.Linq.Expressions;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<Reservation> GetByIdAsync(Guid reservationId, Guid userId);
        Task<List<Reservation>> GetAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[] status, Guid userId);
        Task<bool> AnyAsync(Expression<Func<Reservation, bool>> predicate);
        Task DeleteAsync(Reservation reservation);
    }
}