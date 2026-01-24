namespace BookingSystem.Models
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled, Completed, Expired }
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeople { get; set; }
        public required ReservationStatus Status { get; set; }
        public required Resource Resource { get; set; }

        public Reservation(DateTime startDate, DateTime endDate, Resource resource, int numberOfPeople)
        {
            if (endDate <= startDate) throw new ArgumentException("EndDate must be after StartDate");

            if (startDate < DateTime.Now) throw new ArgumentException("StartDate cannot be in the past");

            Id = Guid.NewGuid();
            Status = ReservationStatus.Pending;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfPeople = numberOfPeople;
            Resource = resource;
        }

        public void ConfirmReservation()
        {
            if (Status != ReservationStatus.Pending) throw new InvalidOperationException("Only pending reservations can be confirmed.");
            if (Status == ReservationStatus.Cancelled) throw new InvalidOperationException("A cancelled reservation cannot be confirmed");

            Resource.BookResource();
            Status = ReservationStatus.Confirmed;
        }

        public void CancelReservation()
        {
            if (Status == ReservationStatus.Cancelled) throw new InvalidOperationException("Reservation is already cancelled.");
            if (Status == ReservationStatus.Completed) throw new InvalidOperationException("A completed reservation cannot be cancelled");

            Status = ReservationStatus.Cancelled;
        }

        public void CompleteReservation()
        {
            if (Status != ReservationStatus.Confirmed) throw new InvalidOperationException("Only confirmed reservations can be completed.");

            Status = ReservationStatus.Completed;
        }

        public void ExpireReservation()
        {
            if (Status != ReservationStatus.Pending) throw new InvalidOperationException("Only pending reservations can be expired.");

            Status = ReservationStatus.Expired;
        }

        public void ExtendReservation(DateTime newEndDate)
        {
            if (newEndDate <= EndDate) throw new ArgumentException("New end date must be after the current end date.");
            if (Status != ReservationStatus.Pending) throw new InvalidOperationException("Only pending reservations can be extended.");

            EndDate = newEndDate;
        }

        public void ChangeNumberOfPeople(int newNumberOfPeople)
        {
            if (newNumberOfPeople <= 0) throw new ArgumentException("Number of people must be greater than zero.");
            if (Status != ReservationStatus.Pending) throw new InvalidOperationException("Only pending reservations can change the number of people.");

            NumberOfPeople = newNumberOfPeople;
        }
    }
}