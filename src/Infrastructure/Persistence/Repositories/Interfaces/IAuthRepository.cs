using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);
    } 
}