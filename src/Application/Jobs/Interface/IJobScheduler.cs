namespace BookingSystem.Application.Jobs.Interface
{
    public interface IJobScheduler
    {
        void ScheduleReservationExpiration(Guid reservationId, string userId);
    }
}