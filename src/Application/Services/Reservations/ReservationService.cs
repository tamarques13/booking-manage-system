using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Application.Jobs.Interface;
using BookingSystem.Application.Mappers;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Domain.Models;

namespace BookingSystem.Application.Services.Reservations
{
    /// <summary>
    /// Application service responsible for managing Reservation lifecycle.
    /// Coordinates repository access and delegates business rules to the domain model.
    /// </summary>

    public class ReservationService(
        IReservationRepository reservationRepository, 
        IReservationCapacity reservationCapacity, 
        IResourceRepository resourceRepository,
        IUserRepository userRepository, 
        IJobScheduler jobScheduler) : IReservationService
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IReservationCapacity _reservationCapacity = reservationCapacity;
        private readonly IResourceRepository _resourceRepository = resourceRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJobScheduler _jobScheduler = jobScheduler;

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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.ValidateReservation(resource);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.AddAsync(reservation);

            _jobScheduler.ScheduleReservationExpiration(reservation.Id, userId);

            return reservation.ToReservationDto(resource, user);
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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.CancelReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.ConfirmReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
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
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.UpdateResource(resourceId);
            reservation.ValidateReservation(resource);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.UpdateDateReservation(newStartDate, newEndDate);
            reservation.ValidateReservation(resource);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, userId);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            reservation.UpdateNumberOfPeople(newNumberOfPeople);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, userId);
            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
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
            var userIds = reservations.Select(r => r.UserId).Distinct().ToList();

            var resources = await _resourceRepository.GetByIdsAsync(resourceIds);
            var users = await _userRepository.GetByIdsAsync(userIds);

            var resourceDictionary = resources.ToDictionary(r => r.Id);
            var userDictionary = users.ToDictionary(u => u.Id);

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                var resource = resourceDictionary[reservation.ResourceId];
                var user = userDictionary[reservation.UserId];

                reservationDtos.Add(reservation.ToReservationDto(resource, user));
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
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            return reservation.ToReservationDto(resource, user);
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
    }
}