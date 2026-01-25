using BookingSystem.Models;
using BookingSystem.DTOs;

namespace BookingSystem.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<ReservationDto> ConfirmReservationAsync(Guid reservationId);
        Task<ReservationDto> CancelReservationAsync(Guid reservationId);
        Task<ReservationDto> CompleteReservationAsync(Guid reservationId);
        Task<ReservationDto> ExpireReservationAsync(Guid reservationId);
        Task<ReservationDto> ExtendReservationAsync(Guid reservationId, DateTime newEndDate);
        Task<ReservationDto> ChangeNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople);
        Task<List<ReservationDto>> GetReservationsAsync();
        Task<ReservationDto> GetReservationByIdAsync(Guid reservationId);
        Task DeleteReservationAsync(Guid reservationId);
    }
}