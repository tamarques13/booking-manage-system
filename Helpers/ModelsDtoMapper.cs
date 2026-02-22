using BookingSystem.DTOs;
using BookingSystem.Models;

namespace BookingSystem.Helpers
{
    public static class ModelsDtoMapper
    {
        public static ReservationDto ToReservationDto(this Reservation reservation, Resource resource, List<ReservationDto>? reservationDtos = null)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                NumberOfPeople = reservation.NumberOfPeople,
                Status = reservation.Status.ToString(),
                CreatedAt = reservation.CreatedAt,
                Resource = new ReservationResourceDto
                {
                    ResourceId = resource.Id,
                    Name = resource.Name,
                    Type = resource.Type.ToString(),
                    OpeningTime = resource.OpeningTime,
                    ClosingTime = resource.ClosingTime,
                    Weekends = resource.Weekends,
                }
            };
        }

        public static ResourceDto ToResourceDto(this Resource resource)
        {
            return new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Capacity = resource.Capacity,
                Type = resource.Type.ToString(),
                Status = resource.Status.ToString(),
                OpeningTime = resource.OpeningTime,
                ClosingTime = resource.ClosingTime,
                Weekends = resource.Weekends,
            };
        }
    }
}