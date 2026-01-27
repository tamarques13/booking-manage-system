using BookingSystem.Models;
using BookingSystem.DTOs;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services.Interfaces;
using BookingSystem.Helpers;

namespace BookingSystem.Services
{
    public class ReservationService(IReservationRepository reservationRepository, IResourceRepository resourceRepository) : IReservationService
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IResourceRepository _resourceRepository = resourceRepository;

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var reservation = Reservation.Create(dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId);

            await _reservationRepository.AddAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> CancelReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.CancelReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> UpdateDateReservationAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.UpdateDateReservation(newStartDate, newEndDate);

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> ChangeNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.ChangeNumberOfPeople(newNumberOfPeople);

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId)
        {
            var reservations = await _reservationRepository.GetAllAsync(resourceId);

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                await reservation.ToReservationDtoAsync(_resourceRepository, isGetAll: true, reservationDtos: reservationDtos);
            }

            return reservationDtos;
        }

        public async Task<ReservationDto> GetReservationByIdAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task DeleteReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            await _reservationRepository.DeleteAsync(reservation);
        }
    }
}