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

        public Resource(string name, int capacity, ResourceType type, TimeOnly openingTime, TimeOnly closingTime)
        {
            if (Status != ResourceStatus.Available) throw new InvalidOperationException("Resource is not available for reservation.");

            Id = Guid.NewGuid();
            Name = name;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            Capacity = capacity;
            Type = type;
            Status = ResourceStatus.Available;
        }

        public void Update(string name, int capacity, ResourceType type, TimeOnly openingTime, TimeOnly closingTime)
        {
            if (!Enum.IsDefined(typeof(ResourceType), type)) throw new ArgumentException($"Invalid resource type: {type}");

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");

            if (capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");

            Name = name;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            Capacity = capacity;
            Type = type;
        }

        public void BookResource()
        {
            if (Status == ResourceStatus.Unavailable) throw new InvalidOperationException("Resource is already booked.");

            Status = ResourceStatus.Unavailable;
        }

        public void ReleaseResource()
        {
            if (Status == ResourceStatus.Available) throw new InvalidOperationException("Resource is already available.");

            Status = ResourceStatus.Available;
        }
    }
}