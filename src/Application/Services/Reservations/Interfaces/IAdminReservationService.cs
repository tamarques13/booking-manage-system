using BookingSystem.Domain.Models;
using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services.Reservations.Interfaces
{
    public interface IAdminReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateAdminReservationDto dto);
        Task<ReservationDto> UpdateReservationAsync(UpdateAdminReservationDto dto, Guid ReservationId);
        Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[] status, string? userId);
        Task<ReservationDto> GetReservationByIdAsync(Guid reservationId);
        Task DeleteReservationByIdAsync(Guid reservationId);
    }
}