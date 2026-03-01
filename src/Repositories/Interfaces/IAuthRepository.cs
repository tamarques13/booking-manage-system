using BookingSystem.Models;

namespace BookingSystem.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);
    } 
}