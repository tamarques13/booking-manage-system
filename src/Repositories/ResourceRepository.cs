using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using BookingSystem.Data;
using BookingSystem.Repositories.Interfaces;

namespace BookingSystem.Repositories
{
    public class ResourceRepository(BookingDbContext context) : IResourceRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task AddAsync(Resource resource)
        {
            _context.Resources.Add(resource);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            _context.Resources.Update(resource);

            await _context.SaveChangesAsync();
        }

        public async Task<Resource> GetByIdAsync(Guid resourceId)
        {
            return await _context.Resources.FindAsync(resourceId) ?? throw new KeyNotFoundException($"Resource with Id {resourceId} not found.");
        }

        public async Task<List<Resource>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids.Count == 0) return new List<Resource>();
            
            IQueryable<Resource> query = _context.Resources;
            
            query = query.Where(r => ids.Contains(r.Id));

             return await _context.Resources.Where(r => ids.Contains(r.Id)).ToListAsync();
        }

        public async Task<List<Resource>> GetAllAsync()
        {
            return await _context.Resources.ToListAsync();
        }

        public async Task DeleteAsync(Resource resource)
        {
            _context.Resources.Remove(resource);

            await _context.SaveChangesAsync();
        }
    }
}