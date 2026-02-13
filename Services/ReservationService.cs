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
    public class ReservationService(IReservationRepository reservationRepository, IResourceRepository resourceRepository) : IReservationService
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IResourceRepository _resourceRepository = resourceRepository;

        /// <summary>
        /// Creates a new reservation for the specified resource.
        /// Validates availability and capacity, persists the reservation
        /// and schedules automatic expiration if not confirmed in time.
        /// </summary>
        /// 
        /// <param name="dto">Input data required to create the reservation.</param>
        /// <returns>The created reservation mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var reservation = new Reservation(dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            ValidateReservation(reservation, resource);

            await ValidateCapacityNotExceeded(reservation, resource);

            await _reservationRepository.AddAsync(reservation);

            BackgroundJob.Schedule<ReservationJob>((job) => job.ExpireReservation(reservation.Id), TimeSpan.FromSeconds(60));

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Cancels an existing reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to cancel.</param>
        /// <returns>The updated reservation as a DTO.</returns>
        ///
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> CancelReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

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
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> ConfirmReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

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
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.UpdateResource(resourceId);

            ValidateReservation(reservation, resource);

            await ValidateCapacityNotExceeded(reservation, resource);

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
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.UpdateDateReservation(newStartDate, newEndDate);

            ValidateReservation(reservation, resource);

            await ValidateCapacityNotExceeded(reservation, resource);

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
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the reservation violates business rules. </exception>

        public async Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.ChangeNumberOfPeople(newNumberOfPeople);

            await ValidateCapacityNotExceeded(reservation, resource);

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
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endtime, ReservationStatus? status)
        {
            var reservations = await _reservationRepository.GetAllAsync(resourceId, startTime, endtime, status);

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

                reservation.ToReservationDto(resource, isGetAll: true, reservationDtos: reservationDtos);
            }

            return reservationDtos;
        }

        /// <summary>
        /// Retrieves a reservation by its ID.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>The reservation as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>

        public async Task<ReservationDto> GetReservationByIdAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            return reservation.ToReservationDto(resource);
        }

        /// <summary>
        /// Deletes a reservation by its ID.
        /// </summary>
        /// <param name="reservationId">The reservation ID to delete.</param>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>

        public async Task DeleteReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            await _reservationRepository.DeleteAsync(reservation);
        }

        ///<summary>
        /// Validates reservation against resource rules such as availability, working hours and weekends.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If any rule is violated.</exception>

        private static void ValidateReservation(Reservation reservation, Resource resource)
        {
            if (resource.Status == ResourceStatus.Unavailable) throw new DomainException("Resource is Unavailable");

            if (!resource.Weekends && (reservation.StartDate.DayOfWeek == DayOfWeek.Saturday || reservation.StartDate.DayOfWeek == DayOfWeek.Sunday
            || reservation.EndDate.DayOfWeek == DayOfWeek.Saturday || reservation.EndDate.DayOfWeek == DayOfWeek.Sunday))
                throw new DomainException("It's not allowed reservations on weekends");

            if (TimeOnly.FromDateTime(reservation.StartDate) < resource.OpeningTime || TimeOnly.FromDateTime(reservation.EndDate) > resource.ClosingTime)
                throw new DomainException("Reservation must be within Resource Hours");
        }

        /// <summary>
        /// Validates that the number of people does not exceed the resource's capacity.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If capacity would be exceeded.</exception>
        
        private async Task ValidateCapacityNotExceeded(Reservation reservation, Resource resource)
        {
            int current = 0;
            int max = 0;

            var Reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, reservation.Status);
            var confrimedReservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, ReservationStatus.Confirmed);
            Reservations.AddRange(confrimedReservations);

            var events = Reservations.SelectMany(r => new[]
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