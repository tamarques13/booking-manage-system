using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.Application.DTOs;

namespace BookingSystem.API.Controllers.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/reservations")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminReservationController(IAdminReservationService reservationService) : ControllerBase
    {
        private readonly IAdminReservationService _reservationService = reservationService;

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateAdminReservation(CreateAdminReservationDto dto)
        {
            var reservationDto = await _reservationService.CreateReservationAsync(dto);
            return CreatedAtAction(nameof(GetAdminReservationsById), new { id = reservationDto.Id }, reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminReservation(UpdateAdminReservationDto dto, Guid id)
        {
            var reservationDto = await _reservationService.UpdateReservationAsync(dto, id);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAdminReservations(string? userId, Guid? ResourceId, DateTime? StartTime, DateTime? EndTime, [FromQuery] ReservationStatus[] status)
        {
            var reservationsDto = await _reservationService.GetReservationsAsync(ResourceId, StartTime, EndTime, status, userId);
            return Ok(reservationsDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminReservationsById(Guid id)
        {
            var reservationDto = await _reservationService.GetReservationByIdAsync(id);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminReservationsById(Guid id)
        {
            await _reservationService.DeleteReservationByIdAsync(id);
            return NoContent();
        }
    }
}