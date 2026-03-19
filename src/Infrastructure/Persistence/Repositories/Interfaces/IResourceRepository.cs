using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IResourceRepository
    {
        Task AddAsync(Resource resource);
        Task UpdateAsync(Resource resource);
        Task<Resource> GetByIdAsync(Guid resourceId);
        Task<List<Resource>> GetByIdsAsync(List<Guid> ids);
        Task<List<Resource>> GetAllAsync();
        Task DeleteAsync(Resource resource);
    }
}