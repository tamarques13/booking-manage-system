using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<List<RefreshToken>> GetAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
        Task DeleteAsync(RefreshToken token);
        Task SaveChangesAsync();
    }
}