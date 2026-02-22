using BookingSystem.Models;
using BookingSystem.DTOs;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services.Interfaces;
using BookingSystem.Helpers;
using BookingSystem.Jobs;
using BookingSystem.ExceptionHelper;
using Hangfire;

namespace BookingSystem.Services
{
    /// <summary>
    /// Application service responsible for managing Reservation lifecycle.
    /// Coordinates repository access and delegates business rules to the domain model.
    /// </summary>
    
    public class ReservationService(IReservationRepository reservationRepository, IResourceRepository resourceRepository) : IReservationService
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IResourceRepository _resourceRepository = resourceRepository;

        /// <summary>
        /// Creates a new reservation for the specified resource.
        /// Validates availability and capacity, persists the reservation
        /// and schedules automatic expiration if not confirmed in time.
        /// 
        /// Once reservation is created, a background job will fire with duration of 
        /// 60 seconds in order for user to confirm the reservation.
        /// </summary>
        /// 
        /// <param name="dto">Input data required to create the reservation.</param>
        /// <returns>The created reservation mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto, string userId)
        {
            var reservation = new Reservation(dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            resource.ValidateReservation(reservation, resource);

            await ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.AddAsync(reservation);

            BackgroundJob.Schedule<ReservationJob>((job) => job.ExpireReservation(reservation.Id, Guid.Parse(userId)), TimeSpan.FromSeconds(60));

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Cancels an existing reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to cancel.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        ///
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> CancelReservationAsync(Guid reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.CancelReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }


        /// <summary>
        /// Confirms an existing reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to confirm.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> ConfirmReservationAsync(Guid reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.ConfirmReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Updates the resource associated with a reservation, validating rules and capacity.
        /// </summary>
        /// <param name="reservationId">The reservation to update.</param>
        /// <param name="resourceId">The new resource ID.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));

            reservation.UpdateResource(resourceId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            resource.ValidateReservation(reservation, resource);
            await ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Updates the start and end dates of a reservation, validating rules and capacity.
        /// </summary>
        /// <param name="reservationId">The reservation to update.</param>
        /// <param name="newStartDate">The new start date.</param>
        /// <param name="newEndDate">The new end date.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.UpdateDateReservation(newStartDate, newEndDate);

            resource.ValidateReservation(reservation, resource);
            await ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Updates the number of people for a reservation, validating capacity.
        /// </summary>
        /// <param name="reservationId">The reservation to update.</param>
        /// <param name="newNumberOfPeople">The new number of people.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.ChangeNumberOfPeople(newNumberOfPeople);

            await ValidateCapacityNotExceeded(reservation, resource, userId);
            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Retrieves reservations based on optional filters.
        /// </summary>
        /// <param name="resourceId">Optional resource ID filter.</param>
        /// <param name="startTime">Optional start time filter.</param>
        /// <param name="endtime">Optional end time filter.</param>
        /// <param name="status">Optional reservation status filter.</param>
        /// <returns>List of reservation DTOs.</returns>

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endtime, ReservationStatus[] status, string userId)
        {
            var reservations = await _reservationRepository.GetAllAsync(resourceId, startTime, endtime, status, Guid.Parse(userId));
            var resourceIds = reservations.Select(r => r.ResourceId).Distinct().ToList();
            var resources = await _resourceRepository.GetByIdsAsync(resourceIds);
            var resourceDictionary = resources.ToDictionary(r => r.Id);

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                var resource = resourceDictionary[reservation.ResourceId];
                reservationDtos.Add(reservation.ToReservationDto(resource, reservationDtos: reservationDtos));
            }

            return reservationDtos;
        }

        /// <summary>
        /// Retrieves a reservation by its ID.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>The reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation or/and resource does not exist. </exception>

        public async Task<ReservationDto> GetReservationByIdAsync(Guid reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Permanently removes the reservation from the system. Should not be used if historical booking data must be preserved.
        /// </summary>
        /// <param name="reservationId">The reservation ID to delete.</param>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the reservation does not exist. </exception>

        public async Task DeleteReservationAsync(Guid reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, Guid.Parse(userId));

            await _reservationRepository.DeleteAsync(reservation);
        }

        /// <summary>
        /// Validates that the number of people does not exceed the resource's capacity.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If capacity would be exceeded.</exception>

        private async Task ValidateCapacityNotExceeded(Reservation reservation, Resource resource, string userId)
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

            foreach (var e in events)
            {
                current += e.delta;
                max = Math.Max(max, current);
            }

            if (max > resource.Capacity) throw new DomainException(resource.Name + " has reached capacity limit.");
        }
    }
}