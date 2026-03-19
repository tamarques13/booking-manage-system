using Microsoft.EntityFrameworkCore;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Infrastructure.Persistence.Configurations;
using BookingSystem.Domain.Models;

namespace BookingSystem.Infrastructure.Persistence.Repositories
{
    public class AuthRepository(BookingDbContext context) : IAuthRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}