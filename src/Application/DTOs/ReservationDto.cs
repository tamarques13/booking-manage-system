using BookingSystem.Domain.Models;

namespace BookingSystem.Application.DTOs
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeople { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ReservationResourceDto Resource { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }

    public class CreateReservationDto
    {
        public int NumberOfPeople { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ResourceId { get; set; }
    }

    public class CreateAdminReservationDto
    {
        public int NumberOfPeople { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ResourceId { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateAdminReservationDto
    {
        public ReservationStatus Status {get; set;}
        public int NumberOfPeople { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ResourceId { get; set; }
        public Guid UserId { get; set; }
    }
}