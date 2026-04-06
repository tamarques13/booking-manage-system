using BookingSystem.Domain.Models;
using System.Linq.Expressions;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces
{
    public interface IAdminReservationRepository
    {
        Task<List<Reservation>> GetAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[]? status, Guid? userId);
        Task<Reservation> GetByIdAsync(Guid reservationId);
    }
}