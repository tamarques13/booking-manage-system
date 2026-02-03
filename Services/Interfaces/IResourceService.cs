using BookingSystem.Models;
using BookingSystem.DTOs;

namespace BookingSystem.Services.Interfaces
{
    public interface IResourceService
    {
        Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto);
        Task<ResourceDto> UpdateResourceAsync(Guid resourceId, CreateResourceDto dto);
        Task<ResourceDto> ActivateResourceAsync(Guid resourceId);
        Task<ResourceDto> DeactivateResourceAsync(Guid resourceId);
        Task<ResourceDto> UpdateWeekendAsync(Guid reservationId);
        Task<List<ResourceDto>> GetResourcesAsync();
        Task<ResourceDto> GetResourceByIdAsync(Guid resourceId);
        Task DeleteResourceAsync(Guid resourceId);
    }
}
