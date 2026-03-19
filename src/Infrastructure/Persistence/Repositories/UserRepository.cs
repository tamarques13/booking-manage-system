using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Infrastructure.Persistence.Configurations;
using BookingSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories
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

        public async Task<List<User>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids.Count == 0) return [];

            IQueryable<User> query = _context.Users;

            query = query.Where(r => ids.Contains(r.Id));

            return await query.ToListAsync();
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