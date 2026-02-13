using BookingSystem.Repositories.Interfaces;
using BookingSystem.Models;


namespace BookingSystem.Jobs
{
    public class ReservationJob(IReservationRepository reservationRepository)
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        public async Task ExpireReservation(Guid Id)
        {
           var reservation = await _reservationRepository.GetByIdAsync(Id);

            if(reservation.Status == ReservationStatus.Confirmed) return;

            reservation.ExpireReservation();

            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}