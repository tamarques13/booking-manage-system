using BookingSystem.Application.Jobs.Interface;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Domain.Models;
using Hangfire;

namespace BookingSystem.Application.Jobs
{
    public class HangfireJobScheduler(IReservationRepository reservationRepository): IJobScheduler
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;

        public void ScheduleReservationExpiration(Guid reservationId, string userId)
        {
            BackgroundJob.Schedule<HangfireJobScheduler>(job => ExpireReservation(reservationId, Guid.Parse(userId)),TimeSpan.FromSeconds(60));
        }

        public async Task ExpireReservation(Guid Id, Guid userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(Id, userId);

            if (reservation.Status == ReservationStatus.Confirmed) return;

            reservation.ExpireReservation();

            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}