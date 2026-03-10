using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Services.Interfaces;
using BookingSystem.Models;
using BookingSystem.DTOs;

namespace BookingSystem.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/reservations")]
    [Authorize(Roles = "Admin")]
    public class AdminReservationController(IReservationService reservationService) : ControllerBase
    {
        private readonly IReservationService _reservationService = reservationService;

        [HttpPost]
        public async Task<IActionResult> CreateAdminReservation(CreateAdminReservationDto dto)
        {
            var reservationDto = await _reservationService.CreateAdminReservationAsync(dto);
            return CreatedAtAction(nameof(GetAdminReservationsById), new { id = reservationDto.Id }, reservationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminReservation(UpdateAdminReservationDto dto, Guid id)
        {
            var reservationDto = await _reservationService.UpdateAdminReservationAsync(dto, id);
            return Ok(reservationDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminReservations(string? userId, Guid? ResourceId, DateTime? StartTime, DateTime? EndTime, [FromQuery] ReservationStatus[] status)
        {
            var reservationsDto = await _reservationService.GetAdminReservationsAsync(ResourceId, StartTime, EndTime, status, userId);
            return Ok(reservationsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminReservationsById(Guid id)
        {
            var reservationDto = await _reservationService.GetAdminReservationByIdAsync(id);
            return Ok(reservationDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminReservationsById(Guid id)
        {
            await _reservationService.DeleteAdminReservationByIdAsync(id);
            return NoContent();
        }
    }
}