using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BookingSystem.Helpers
{
    public static class ReservationMapper
    {
        public static async Task<ReservationDto> ToReservationDtoAsync(this Reservation reservation, IResourceRepository resourceRepository, bool isGetAll = false, List<ReservationDto>? reservationDtos = null)
        {
            var resource = await resourceRepository.GetByIdAsync(reservation.ResourceId);

            var reservationDto = new ReservationDto
            {
                Id = reservation.Id,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                NumberOfPeople = reservation.NumberOfPeople,
                Status = reservation.Status.ToString(),
                Resource = new ReservationResourceDto
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Type = resource.Type.ToString(),
                    Status = resource.Status.ToString()
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
    }
}