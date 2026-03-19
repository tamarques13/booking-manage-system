using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Models
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled, Completed, Expired }
    public class Reservation
    {
        public Reservation() { }
        public Guid Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int NumberOfPeople { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid ResourceId { get; private set; }
        public Resource? Resource { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }

        public Reservation(DateTime startDate, DateTime endDate, int numberOfPeople, Guid resourceId, Guid userId)
        {
            if (endDate <= startDate) throw new DomainException("EndDate must be after StartDate");

            if (startDate < DateTime.Now) throw new DomainException("StartDate cannot be in the past");

            if (numberOfPeople <= 0) throw new DomainException("NumberOfPeople must be greater than zero");


            Id = Guid.NewGuid();
            Status = ReservationStatus.Pending;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfPeople = numberOfPeople;
            CreatedAt = DateTime.UtcNow;
            ResourceId = resourceId;
            UserId = userId;
        }

        public void Update(ReservationStatus status, DateTime startDate, DateTime endDate, int numberOfPeople, Guid resourceId, Guid userId)
        {
            if(!Enum.IsDefined(typeof(ReservationStatus), status)) throw new DomainException($"Status: {status} is not allowed");

            if (endDate <= startDate) throw new DomainException("EndDate must be after StartDate");

            if (startDate < DateTime.Now) throw new DomainException("StartDate cannot be in the past");

            if (numberOfPeople <= 0) throw new DomainException("NumberOfPeople must be greater than zero");

            Status = status;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfPeople = numberOfPeople;
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

        public void ExpireReservation()
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can be expired.");

            Status = ReservationStatus.Expired;
        }

        public void UpdateDateReservation(DateTime newStartDate, DateTime newEndDate)
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can be confirmed.");
            if (newStartDate < DateTime.Now) throw new DomainException("StartDate cannot be in the past");
            if (newEndDate <= newStartDate) throw new DomainException("EndDate must be after StartDate");

            StartDate = newStartDate;
            EndDate = newEndDate;
        }

        public void UpdateNumberOfPeople(int newNumberOfPeople)
        {

            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can change the number of people.");
            if (newNumberOfPeople <= 0) throw new DomainException("NumberOfPeople must be greater than zero");

            NumberOfPeople = newNumberOfPeople;
        }

        public void UpdateResource(Guid resourceId)
        {
            if (Status != ReservationStatus.Pending) throw new DomainException("Only pending reservations can change the number of people.");

            ResourceId = resourceId;
        }

        ///<summary>
        /// Validates reservation against resource rules such as availability, working hours and weekends.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If any rule is violated.</exception>

        public void ValidateReservation(Resource resource)
        {
            if (resource.Status == ResourceStatus.Unavailable) throw new DomainException("Resource is Unavailable");

            if (!resource.Weekends && (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday
            || EndDate.DayOfWeek == DayOfWeek.Saturday || EndDate.DayOfWeek == DayOfWeek.Sunday))
                throw new DomainException("It's not allowed reservations on weekends");

            if (TimeOnly.FromDateTime(StartDate) < resource.OpeningTime || TimeOnly.FromDateTime(EndDate) > resource.ClosingTime)
                throw new DomainException("Reservation must be within Resource Hours");
        }
    }
}