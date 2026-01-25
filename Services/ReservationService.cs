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

        public async Task<ReservationDto> ConfirmReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.ConfirmReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> CancelReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.CancelReservation();
            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> CompleteReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.CompleteReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> ExpireReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.ExpireReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return await reservation.ToReservationDtoAsync(_resourceRepository);
        }

        public async Task<ReservationDto> ExtendReservationAsync(Guid reservationId, DateTime newEndDate)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.ExtendReservation(newEndDate);

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

        public async Task<List<ReservationDto>> GetReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

                reservationDtos.Add(new ReservationDto
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
                });
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