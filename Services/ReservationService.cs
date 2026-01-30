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
            var reservation = new Reservation(dto.StartDate, dto.EndDate, dto.NumberOfPeople, dto.ResourceId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            ValidateCreateReservation(reservation, resource);

            await _reservationRepository.AddAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> CancelReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.CancelReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            reservation.UpdateResource(resourceId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            if (TimeOnly.FromDateTime(newStartDate) <= resource.OpeningTime || TimeOnly.FromDateTime(newEndDate) >= resource.ClosingTime)
                throw new InvalidOperationException("Reservation must be within Resource Hours");

            reservation.UpdateDateReservation(newStartDate, newEndDate);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.ChangeNumberOfPeople(newNumberOfPeople);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId)
        {
            var reservations = await _reservationRepository.GetAllAsync(resourceId);

            var reservationDtos = new List<ReservationDto>();

            foreach (var reservation in reservations)
            {
                var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

                reservation.ToReservationDto(resource, isGetAll: true, reservationDtos: reservationDtos);
            }

            return reservationDtos;
        }

        public async Task<ReservationDto> GetReservationByIdAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            return reservation.ToReservationDto(resource);
        }

        public async Task DeleteReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            await _reservationRepository.DeleteAsync(reservation);
        }

        private static void ValidateCreateReservation(Reservation reservation, Resource resource)
        {
            if (resource.Status == ResourceStatus.Unavailable) throw new InvalidOperationException("Resource is Unavailable");

            if (!resource.Weekends && (reservation.StartDate.DayOfWeek == DayOfWeek.Saturday || reservation.StartDate.DayOfWeek == DayOfWeek.Sunday))
                throw new InvalidOperationException("It's not allowed reservations on weekends");

            if (TimeOnly.FromDateTime(reservation.StartDate) <= resource.OpeningTime || TimeOnly.FromDateTime(reservation.EndDate) >= resource.ClosingTime)
                throw new InvalidOperationException("Reservation must be within Resource Hours");
        }
    }
}