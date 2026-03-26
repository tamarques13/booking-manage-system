using BookingSystem.Domain.Models;
using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services.Reservations.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto, string userId);
        Task<ReservationDto> CancelReservationAsync(Guid reservationId, string userId);
        Task<ReservationDto> ConfirmReservationAsync(Guid reservationId, string userId);
        Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId, string userId);
        Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate, string userId);
        Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople, string userId);
        Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[] status, string userId);
        Task<ReservationDto> GetReservationByIdAsync(Guid reservationId, string userId);
        Task DeleteReservationAsync(Guid reservationId, string userId);
    }
}