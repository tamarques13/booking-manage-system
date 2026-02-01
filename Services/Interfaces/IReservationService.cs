using BookingSystem.Models;
using BookingSystem.DTOs;

namespace BookingSystem.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<ReservationDto> CancelReservationAsync(Guid reservationId);
        Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId);
        Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate);
        Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople);
        Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime);
        Task<ReservationDto> GetReservationByIdAsync(Guid reservationId);
        Task DeleteReservationAsync(Guid reservationId);
    }
}