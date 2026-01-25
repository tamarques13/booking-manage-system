using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using BookingSystem.Data;
using BookingSystem.Repositories.Interfaces;

namespace BookingSystem.Repositories
{
    public class ReservationRepository(BookingDbContext context) : IReservationRepository
    {
        private readonly BookingDbContext _context = context;

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);

            await _context.SaveChangesAsync();
        }

        public async Task<Reservation> GetByIdAsync(Guid reservationId)
        {
            return await _context.Reservations.FindAsync(reservationId) ?? throw new KeyNotFoundException($"Reservation with Id {reservationId} not found.");
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

        }
    }
}