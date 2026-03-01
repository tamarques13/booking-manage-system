using BookingSystem.Jobs.Interface;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Models;
using Hangfire;

namespace BookingSystem.Jobs
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