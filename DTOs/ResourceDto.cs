namespace BookingSystem.DTOs
{
    public class ResourceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public bool Weekends { get; set; }
    }

    public class CreateResourceDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Type { get; set; } = string.Empty;
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public bool Weekends { get; set; }
    }

    public class ReservationResourceDto
    {
        public Guid ResourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public bool Weekends { get; set; }
    }
}