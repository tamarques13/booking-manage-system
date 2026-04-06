using BookingSystem.Domain.Models;

namespace BookingSystem.Application.Services.Reservations.Interfaces
{
    public interface IReservationCapacity
    {
        Task ValidateCapacityNotExceeded(Reservation reservation, Resource resource, string userId);
    }
}