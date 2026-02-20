using BookingSystem.Models;

namespace BookingSystem.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<Reservation> GetByIdAsync(Guid reservationId);
        Task<List<Reservation>> GetAllAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[]? status);
        Task DeleteAsync(Reservation reservation);
    }
}