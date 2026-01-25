namespace BookingSystem.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfPeople { get; set; }
    public string Status { get; set; } = string.Empty;
    public ReservationResourceDto Resource { get; set; } = null!;
}

public class CreateReservationDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfPeople { get; set; }
    public Guid ResourceId { get; set; }
}