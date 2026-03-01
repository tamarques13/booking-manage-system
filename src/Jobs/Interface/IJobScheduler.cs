namespace BookingSystem.Jobs.Interface
{
    public interface IJobScheduler
    {
        void ScheduleReservationExpiration(Guid reservationId, string userId);
    }
}