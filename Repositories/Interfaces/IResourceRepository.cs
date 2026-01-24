using BookingSystem.Models;

namespace BookingSystem.Repositories.Interfaces
{
    public interface IResourceRepository
    {
        Task AddAsync(Resource resource);
        Task UpdateAsync(Resource resource);
        Task<Resource> GetByIdAsync(Guid resourceId);
        Task<List<Resource>> GetAllAsync();
        Task DeleteAsync(Resource resource);
    }
}