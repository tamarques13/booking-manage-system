using BookingSystem.ExceptionHelper;

namespace BookingSystem.Models
{
    public enum ResourceType { Office, ConferenceRoom, MeetingRoom }
    public enum ResourceStatus { Available, Unavailable }
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public ResourceType Type { get; set; }
        public ResourceStatus Status { get; set; }
        public bool Weekends { get; set; }

        public Resource(string name, int capacity, ResourceType type, TimeOnly openingTime, TimeOnly closingTime, bool weekends)
        {
            if (Status != ResourceStatus.Available) throw new DomainException("Resource is not available for reservation.");

            Id = Guid.NewGuid();
            Name = name;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            Capacity = capacity;
            Type = type;
            Status = ResourceStatus.Available;
            Weekends = weekends;
        }

        public void Update(string name, int capacity, ResourceType type, TimeOnly openingTime, TimeOnly closingTime)
        {
            if (!Enum.IsDefined(typeof(ResourceType), type)) throw new DomainException($"Invalid resource type: {type}");

            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name cannot be empty.");

            if (capacity <= 0) throw new DomainException("Capacity must be greater than zero.");

            Name = name;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            Capacity = capacity;
            Type = type;
        }

        public void ActivateResource()
        {
            if (Status == ResourceStatus.Available) throw new DomainException("Resource is already available.");

            Status = ResourceStatus.Available;
        }

        public void DeactivateResource()
        {
            if (Status == ResourceStatus.Unavailable) throw new DomainException("Resource is already booked.");

            Status = ResourceStatus.Unavailable;
        }

        public void UpdateWeekend()
        {
            Weekends = !Weekends;
        }

        ///<summary>
        /// Validates reservation against resource rules such as availability, working hours and weekends.
        /// </summary>
        /// <param name="reservation">The reservation to validate.</param>
        /// <param name="resource">The resource being reserved.</param>
        /// 
        /// <exception cref="DomainException">If any rule is violated.</exception>

        public void ValidateReservation(Reservation reservation, Resource resource)
        {
            if (resource.Status == ResourceStatus.Unavailable) throw new DomainException("Resource is Unavailable");

            if (!resource.Weekends && (reservation.StartDate.DayOfWeek == DayOfWeek.Saturday || reservation.StartDate.DayOfWeek == DayOfWeek.Sunday
            || reservation.EndDate.DayOfWeek == DayOfWeek.Saturday || reservation.EndDate.DayOfWeek == DayOfWeek.Sunday))
                throw new DomainException("It's not allowed reservations on weekends");

            if (TimeOnly.FromDateTime(reservation.StartDate) < resource.OpeningTime || TimeOnly.FromDateTime(reservation.EndDate) > resource.ClosingTime)
                throw new DomainException("Reservation must be within Resource Hours");
        }
    }
}