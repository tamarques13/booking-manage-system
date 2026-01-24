namespace BookingSystem.Models
{
    public enum ResourceType { Office, ConferenceRoom, MeetingRoom }
    public enum ResourceStatus { Available, Unavailable }
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; private set; }
        public ResourceType Type { get; set; }
        public ResourceStatus Status { get; set; }

        public Resource(string name, int capacity, ResourceType type)
        {
            if (Status != ResourceStatus.Available) throw new InvalidOperationException("Resource is not available for reservation.");

            Id = Guid.NewGuid();
            Name = name;
            Capacity = capacity;
            Type = type;
            Status = ResourceStatus.Available;
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