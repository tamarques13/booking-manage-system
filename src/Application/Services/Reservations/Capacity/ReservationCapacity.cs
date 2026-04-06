using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Domain.Models;

namespace BookingSystem.Application.Services.Reservations.Capacity
{
    /// <summary>
    /// Provides capacity validation logic for reservations.
    /// This service ensures that a reservation does not exceed the capacity of the
    /// associated resource. It aggregates all existing reservations for the same
    /// resource within the specified time range considering only confirmed and
    /// pending reservation.
    /// </summary>

    public class ReservationCapacity(IReservationRepository reservationRepository) : IReservationCapacity
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;

        /// <summary>
        /// Validates that the number of people does not exceed the resource's capacity.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If capacity would be exceeded.</exception>

        public async Task ValidateCapacityNotExceeded(Reservation reservation, Resource resource, string userId)
        {
            int current = 0;
            int max = 0;

            var Reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, Guid.Parse(userId));
            var allReservations = Reservations.Append(reservation);

            var events = allReservations.SelectMany(r => new[]
            {
                (time: r.StartDate, delta: r.NumberOfPeople),
                (time: r.EndDate, delta: -r.NumberOfPeople)
            })
            .OrderBy(e => e.time).ThenBy(e => e.delta);

            foreach (var (time, delta) in events)
            {
                current += delta;
                max = Math.Max(max, current);
            }

            if (max > resource.Capacity) throw new DomainException(resource.Name + " has reached capacity limit.");
        }
    }
}