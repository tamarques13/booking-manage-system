using BookingSystem.Repositories.Interfaces;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Repositories
{
    public class UserRepository(BookingDbContext dbContext) : IUserRepository
    {
        private readonly BookingDbContext _context = dbContext;
        public async Task UpdateByIdAsync(User user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId) ?? throw new KeyNotFoundException($"Resource with Id {userId} not found.");
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task DeleteByIdAsync(User user)
        {
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}