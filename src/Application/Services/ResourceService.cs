using BookingSystem.Domain.Models;
using BookingSystem.Application.Services.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Mappers;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Application.Services
{

    /// <summary>
    /// Application service responsible for managing Resource lifecycle.
    /// Coordinates repository access and delegates business rules to the domain model.
    /// </summary>

    public class ResourceService(IResourceRepository resourceRepository, IReservationRepository reservationRepository) : IResourceService
    {
        private readonly IResourceRepository _resourceRepository = resourceRepository;
        private readonly IReservationRepository _reservationRepository = reservationRepository;

        /// <summary>
        /// Creates a new Resource to be used by a reservation.
        /// </summary>
        /// 
        /// <param name="dto">Input data required to create the Resource.</param>
        /// <returns>The created resource mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the resource violates business rules. </exception>

        public async Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto)
        {
            var resource = new Resource(dto.Name, dto.Capacity, Enum.Parse<ResourceType>(dto.Type), dto.OpeningTime, dto.ClosingTime, dto.Weekends);

            await _resourceRepository.AddAsync(resource);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Updates the core configuration of the resource (name, capacity, type and operating hours).
        /// </summary>
        /// 
        /// <param name="resourceId">The resource to update.</param>
        /// <param name="dto">Input data required to update the Resource.</param>
        /// <returns>The updated resource mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the resource violates business rules. </exception>

        public async Task<ResourceDto> UpdateResourceAsync(Guid resourceId, CreateResourceDto dto)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            ArgumentNullException.ThrowIfNull(dto);

            resource.Update(dto.Name, dto.Capacity, Enum.Parse<ResourceType>(dto.Type), dto.OpeningTime, dto.ClosingTime);

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Activates a resource so it becomes available for bookings.
        /// </summary>
        /// 
        /// <param name="resourceId">The Id of the resource to activate.</param>
        /// <returns>The updated resource mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the resource violates business rules. </exception>

        public async Task<ResourceDto> ActivateResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            resource.ActivateResource();

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Deactivates a resource so it becomes unavailable for bookings.
        /// </summary>
        /// 
        /// <param name="resourceId">The Id of the resource to deactivate.</param>
        /// <returns>The updated resource mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the resource violates business rules. </exception>

        public async Task<ResourceDto> DeactivateResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            resource.DeactivateResource();

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Toggles the weekend availability of the resource. Used to control whether bookings are allowed on Saturdays and Sundays.
        /// </summary>
        /// 
        /// <param name="resourceId">The Id of the resource to update.</param>
        /// <returns>The updated resource mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>
        /// <exception cref="DomainException">Thrown when the resource violates business rules. </exception>

        public async Task<ResourceDto> UpdateWeekendAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            resource.UpdateWeekend();

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Retrieves all resources.
        /// </summary>
        /// 
        /// <returns>List of resources DTOs.</returns>

        public async Task<List<ResourceDto>> GetResourcesAsync()
        {
            var resources = await _resourceRepository.GetAllAsync();

            var ResourcesList = resources.Select(resource => resource.ToResourceDto()).ToList();

            return ResourcesList;
        }

        /// <summary>
        /// Retrieves a resource by its ID.
        /// </summary>
        /// <param name="resourceId">The resource ID.</param>
        /// <returns>The resource as a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>

        public async Task<ResourceDto> GetResourceByIdAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            return resource.ToResourceDto();
        }

        /// <summary>
        /// Permanently removes the resource from the system. Should not be used if historical booking data must be preserved.
        /// </summary>
        /// <param name="resourceId">The resource ID to delete.</param>
        /// 
        /// <exception cref="KeyNotFoundException"> Thrown when the resource does not exist. </exception>

        public async Task DeleteResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            var hasReservations = await _reservationRepository.AnyAsync(r => r.ResourceId == resourceId);

            if (hasReservations) throw new DomainException("This item cannot be modified.");

            await _resourceRepository.DeleteAsync(resource);
        }
    }
}
