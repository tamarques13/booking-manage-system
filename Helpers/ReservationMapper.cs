using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BookingSystem.Helpers
{
    public static class ReservationMapper
    {
        public static async Task<ReservationDto> ToReservationDtoAsync(this Reservation reservation, IResourceRepository resourceRepository)
        {
            var resource = await resourceRepository.GetByIdAsync(reservation.ResourceId);
            
            return new ReservationDto
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
        }
    }
}