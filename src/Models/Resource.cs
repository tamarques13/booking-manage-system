using BookingSystem.ExceptionHelper;

namespace BookingSystem.Models
{
    public enum ResourceType { Office, ConferenceRoom, MeetingRoom }
    public enum ResourceStatus { Available, Unavailable }
    public class Resource
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Capacity { get; private set; }
        public TimeOnly OpeningTime { get; private set; }
        public TimeOnly ClosingTime { get; private set; }
        public ResourceType Type { get; private set; }
        public ResourceStatus Status { get; private set; }
        public bool Weekends { get; private set; }
        public ICollection<Reservation> Reservations {get; private set;} = [];

        public Resource(string name, int capacity, ResourceType type, TimeOnly openingTime, TimeOnly closingTime, bool weekends)
        {
            if (Status != ResourceStatus.Available) throw new DomainException("Resource is not available for reservation.");
            
            if (closingTime == TimeOnly.FromTimeSpan(TimeSpan.Zero)) throw new DomainException("Closing time cannot be 00:00:00.");

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

            if (closingTime == TimeOnly.FromTimeSpan(TimeSpan.Zero)) throw new DomainException("Closing time cannot be 00:00:00.");

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
    }
}