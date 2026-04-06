using BookingSystem.Domain.Models;
using BookingSystem.Application.DTOs;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Application.Mappers;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Application.Services.Reservations
{
    /// <summary>
    /// Application service responsible for managing Reservation lifecycle.
    /// Coordinates repository access and delegates business rules to the domain model.
    /// </summary>

    public class AdminReservationService(
        IReservationRepository reservationRepository,
        IAdminReservationRepository adminReservationRepository,
        IReservationCapacity reservationCapacity,
        IResourceRepository resourceRepository,
        IUserRepository userRepository) : IAdminReservationService
    {

        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IAdminReservationRepository _adminReservationRepository = adminReservationRepository;
        private readonly IReservationCapacity _reservationCapacity = reservationCapacity; 
        private readonly IResourceRepository _resourceRepository = resourceRepository;
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Creates a new reservation on behalf of an admin user.
        /// </summary>
        /// <param name="dto">The data transfer object containing reservation details.</param>
        /// <exception cref="ArgumentException">Thrown if reservation validation fails.</exception>

        public async Task<ReservationDto> CreateReservationAsync(CreateAdminReservationDto dto)
        {
            var reservation = new Reservation(dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId, dto.UserId);
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            reservation.ValidateReservation(resource);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, dto.UserId.ToString());

            reservation.ConfirmReservation();

            await _reservationRepository.AddAsync(reservation);

            return reservation.ToReservationDto(resource, user);
        }

        /// <summary>
        /// Updates an existing reservation as an admin user.
        /// </summary>
        /// <param name="dto">The data transfer object containing updated reservation details.</param>
        /// <param name="reservationId">The ID of the reservation to update.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the reservation does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown if reservation validation fails.</exception>

        public async Task<ReservationDto> UpdateReservationAsync(UpdateAdminReservationDto dto, Guid reservationId)
        {
            var reservation = await _adminReservationRepository.GetByIdAsync(reservationId);
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            reservation.Update(dto.Status, dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId, dto.UserId);

            await _reservationCapacity.ValidateCapacityNotExceeded(reservation, resource, dto.UserId.ToString());
            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource, user);
        }

        /// <summary>
        /// Retrieves a list of reservations based on admin level filters.
        /// </summary>
        /// <param name="resourceId">Optional resource ID to filter reservations.</param>
        /// <param name="startTime">Optional start time to filter reservations.</param>
        /// <param name="endTime">Optional end time to filter reservations.</param>
        /// <param name="status">Optional array of reservation statuses to filter.</param>
        /// <param name="userId">Optional user ID to filter reservations.</param>

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endTime, ReservationStatus[]? status, string? userId)
        {
            Guid? id = userId != null ? Guid.Parse(userId) : null;

            var reservations = await _adminReservationRepository.GetAllAsync(resourceId, startTime, endTime, status, id);

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
        /// Retrieves a single reservation by its ID with full details.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to retrieve.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the reservation does not exist.</exception>

        public async Task<ReservationDto> GetReservationByIdAsync(Guid reservationId)
        {
            var reservation = await _adminReservationRepository.GetByIdAsync(reservationId);
            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);
            var user = await _userRepository.GetByIdAsync(reservation.UserId);

            return reservation.ToReservationDto(resource, user);
        }

        /// <summary>
        /// Permanently deletes a reservation by its ID at the admin level. Should not be used if historical booking data must be preserved.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the reservation does not exist.</exception>
        public async Task DeleteReservationByIdAsync(Guid reservationId)
        {
            var reservation = await _adminReservationRepository.GetByIdAsync(reservationId);

            await _reservationRepository.DeleteAsync(reservation);
        }
    }
}