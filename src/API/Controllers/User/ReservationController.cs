using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.API.Controllers.Base;

namespace BookingSystem.API.Controllers.User
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reservations")]
    [Authorize]
    [ApiController]
    public class ReservationController(IReservationService reservationService) : BaseController
    {
        private readonly IReservationService _reservationService = reservationService;

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationDto dto)
        {

            var reservationDto = await _reservationService.CreateReservationAsync(dto, UserId);
            return CreatedAtAction(nameof(GetReservationById), new { id = reservationDto.Id }, reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            var reservationDto = await _reservationService.CancelReservationAsync(id, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/date")]
        public async Task<IActionResult> ExtendReservation(Guid id, DateTime newStartDate, DateTime newEndDate)
        {
            var reservationDto = await _reservationService.UpdateDateAsync(id, newStartDate, newEndDate, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/guest")]
        public async Task<IActionResult> UpdateNumberOfPeople(Guid id, int newNumberOfPeople)
        {
            var reservationDto = await _reservationService.UpdateNumberOfPeopleAsync(id, newNumberOfPeople, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/resource")]
        public async Task<IActionResult> UpdateResource(Guid id, Guid resourceId)
        {
            var reservationDto = await _reservationService.UpdateResourceAsync(id, resourceId, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> ConfirmReservation(Guid id)
        {
            var reservationDto = await _reservationService.ConfirmReservationAsync(id, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetReservations(Guid? ResourceId, DateTime? StartTime, DateTime? EndTime, [FromQuery] ReservationStatus[] status)
        {
            var reservations = await _reservationService.GetReservationsAsync(ResourceId, StartTime, EndTime, status, UserId);
            return Ok(reservations);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var reservationDto = await _reservationService.GetReservationByIdAsync(id, UserId);
            return Ok(reservationDto);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            await _reservationService.DeleteReservationAsync(id, UserId);
            return NoContent();
        }
    }
}