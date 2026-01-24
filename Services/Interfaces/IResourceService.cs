using BookingSystem.Models;
using BookingSystem.DTOs;

namespace BookingSystem.Services.Interfaces
{
    public interface IResourceService
    {
        Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto);
        Task<ResourceDto> BookResourceAsync(Guid resourceId);
        Task<ResourceDto> ReleaseResourceAsync(Guid resourceId);
        Task<List<ResourceDto>> GetResourcesAsync();
        Task<ResourceDto> GetResourceByIdAsync(Guid resourceId);
        Task DeleteResourceAsync(Guid resourceId);
    }
}
