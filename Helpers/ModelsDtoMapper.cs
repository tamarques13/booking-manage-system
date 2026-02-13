using BookingSystem.DTOs;
using BookingSystem.Models;

namespace BookingSystem.Helpers
{
    public static class ModelsDtoMapper
    {
        public static ReservationDto ToReservationDto(this Reservation reservation, Resource resource, bool isGetAll = false, List<ReservationDto>? reservationDtos = null)
        {
            var reservationDto = new ReservationDto
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

            if (isGetAll)
            {
                if (reservationDtos == null) throw new ArgumentNullException(nameof(reservationDtos), "You must provide a list when executing GetAll.");

                reservationDtos.Add(reservationDto);
                return null!;
            }

            return reservationDto;
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