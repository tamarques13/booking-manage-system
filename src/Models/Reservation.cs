using BookingSystem.ExceptionHelper;

namespace BookingSystem.Models
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled, Completed, Expired }
    public class Reservation
    {
        public Reservation() { }

        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeople { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ResourceId { get; set; }
        public Guid UserId { get; set; }

        public Reservation(DateTime startDate, DateTime endDate, int numberOfPeople, Guid resourceId, Guid userId)
        {
            if (endDate <= startDate) throw new ArgumentException("EndDate must be after StartDate");

            if (startDate < DateTime.Now) throw new ArgumentException("StartDate cannot be in the past");

            if (numberOfPeople <= 0) throw new ArgumentException("NumberOfPeople must be greater than zero");

            Id = Guid.NewGuid();
            Status = ReservationStatus.Pending;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfPeople = numberOfPeople;
            CreatedAt = DateTime.UtcNow;
            ResourceId = resourceId;
            UserId = userId;
        }

        public void ConfirmReservation()
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can be confirmed.");
            if (Status == ReservationStatus.Cancelled) throw new DomainException("A cancelled reservation cannot be confirmed");

            Status = ReservationStatus.Confirmed;
        }

        public void CancelReservation()
        {
            if (Status == ReservationStatus.Cancelled) throw new DomainException("Reservation is already cancelled.");
            if (Status == ReservationStatus.Completed) throw new DomainException("A completed reservation cannot be cancelled");

            Status = ReservationStatus.Cancelled;
        }

        public void CompleteReservation()
        {
            if (Status != ReservationStatus.Confirmed) throw new DomainException("Only confirmed reservations can be completed.");

            Status = ReservationStatus.Completed;
        }

        public void ExpireReservation()
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can be expired.");

            Status = ReservationStatus.Expired;
        }

        public void UpdateDateReservation(DateTime newStartDate, DateTime newEndDate)
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can be confirmed.");
            if (newStartDate < DateTime.Now) throw new ArgumentException("StartDate cannot be in the past");
            if (newEndDate <= newStartDate) throw new ArgumentException("EndDate must be after StartDate");

            StartDate = newStartDate;
            EndDate = newEndDate;
        }

        public void ChangeNumberOfPeople(int newNumberOfPeople)
        {

            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can change the number of people.");
            if (newNumberOfPeople <= 0) throw new ArgumentException("Number of people must be greater than zero.");

            NumberOfPeople = newNumberOfPeople;
        }

        public void UpdateResource(Guid resourceId)
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can change the number of people.");

            ResourceId = resourceId;
        }
    }
}