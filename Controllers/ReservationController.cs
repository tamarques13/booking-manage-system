using Microsoft.AspNetCore.Mvc;
using BookingSystem.DTOs;
using BookingSystem.Services;
using BookingSystem.Services.Interfaces;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController(IReservationService reservationService) : ControllerBase
    {
        private readonly IReservationService _reservationService = reservationService;

        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationDto dto)
        {
            var reservationDto = await _reservationService.CreateReservationAsync(dto);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmReservation(Guid id)
        {
            var reservationDto = await _reservationService.ConfirmReservationAsync(id);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            var reservationDto = await _reservationService.CancelReservationAsync(id);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteReservation(Guid id)
        {
            var reservationDto = await _reservationService.CompleteReservationAsync(id);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/expire")]
        public async Task<IActionResult> ExpireReservation(Guid id)
        {
            var reservationDto = await _reservationService.ExpireReservationAsync(id);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/extend")]
        public async Task<IActionResult> ExtendReservation(Guid id, DateTime newEndDate)
        {
            var reservationDto = await _reservationService.ExtendReservationAsync(id, newEndDate);
            return Ok(reservationDto);
        }

        [HttpPut("{id}/change-people")]
        public async Task<IActionResult> ChangeNumberOfPeople(Guid id, int newNumberOfPeople)
        {
            var reservationDto = await _reservationService.ChangeNumberOfPeopleAsync(id, newNumberOfPeople);
            return Ok(reservationDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _reservationService.GetReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var reservationDto = await _reservationService.GetReservationByIdAsync(id);
            return Ok(reservationDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            await _reservationService.DeleteReservationAsync(id);
            return NoContent();
        }
    }
}