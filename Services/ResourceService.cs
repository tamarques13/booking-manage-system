using BookingSystem.Models;
using BookingSystem.Services.Interfaces;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.DTOs;
using BookingSystem.Helpers;

namespace BookingSystem.Services
{
    public class ResourceService(IResourceRepository resourceRepository) : IResourceService
    {
        private readonly IResourceRepository _resourceRepository = resourceRepository;

        public async Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto)
        {
            var resource = new Resource(dto.Name, dto.Capacity, Enum.Parse<ResourceType>(dto.Type), dto.OpeningTime, dto.ClosingTime);

            await _resourceRepository.AddAsync(resource);

            return resource.ToResourceDto();
        }

        public async Task<ResourceDto> UpdateResourceAsync(Guid resourceId, CreateResourceDto dto)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            ArgumentNullException.ThrowIfNull(dto);

            resource.Update(dto.Name, dto.Capacity, Enum.Parse<ResourceType>(dto.Type), dto.OpeningTime, dto.ClosingTime);

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        public async Task<ResourceDto> ActivateResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            resource.BookResource();

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        public async Task<ResourceDto> DeactivateResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            resource.ReleaseResource();

            await _resourceRepository.UpdateAsync(resource);

            return resource.ToResourceDto();
        }

        public async Task<List<ResourceDto>> GetResourcesAsync()
        {
            var resources = await _resourceRepository.GetAllAsync();

            return resources.Select(resource => new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Capacity = resource.Capacity,
                Type = resource.Type.ToString(),
                Status = resource.Status.ToString()
            }).ToList();
        }

        public async Task<ResourceDto> GetResourceByIdAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            return resource.ToResourceDto();
        }

        public async Task DeleteResourceAsync(Guid resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);

            await _resourceRepository.DeleteAsync(resource);
        }
    }
}
