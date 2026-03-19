using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IUserRepository
    {
       Task UpdateByIdAsync(User user);
       Task<User> GetByIdAsync(Guid userId);
       Task<List<User>> GetByIdsAsync(List<Guid> ids);
       Task<List<User>> GetAllAsync();
       Task DeleteByIdAsync(User user);
    }
}