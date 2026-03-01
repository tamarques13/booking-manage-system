using Microsoft.EntityFrameworkCore;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Data;
using BookingSystem.Models;

namespace BookingSystem.Repositories
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