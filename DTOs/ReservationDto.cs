namespace BookingSystem.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfPeople { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ReservationResourceDto Resource { get; set; } = null!;
}

public class CreateReservationDto
{
    public int NumberOfPeople { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ResourceId { get; set; }
}