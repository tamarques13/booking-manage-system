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

            var reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, reservation.Status);

            ValidateReservation(reservation, resource);

            ValidateCapacityNotExceeded(reservation, resource, reservations);

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

        public async Task<ReservationDto> ConfirmReservationAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            reservation.ConfirmReservation();

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateResourceAsync(Guid reservationId, Guid resourceId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            var reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, reservation.Status);

            reservation.UpdateResource(resourceId);

            ValidateReservation(reservation, resource);

            ValidateCapacityNotExceeded(reservation, resource, reservations);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateDateAsync(Guid reservationId, DateTime newStartDate, DateTime newEndDate)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);

            var reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, reservation.Status);

            reservation.UpdateDateReservation(newStartDate, newEndDate);

            ValidateReservation(reservation, resource);

            ValidateCapacityNotExceeded(reservation, resource, reservations);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<ReservationDto> UpdateNumberOfPeopleAsync(Guid reservationId, int newNumberOfPeople)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            var resource = await _resourceRepository.GetByIdAsync(reservation.ResourceId);
            
            var reservations = await _reservationRepository.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, reservation.Status);

            reservation.ChangeNumberOfPeople(newNumberOfPeople);

            ValidateCapacityNotExceeded(reservation, resource, reservations);

            await _reservationRepository.UpdateAsync(reservation);

            return reservation.ToReservationDto(resource);
        }

        public async Task<List<ReservationDto>> GetReservationsAsync(Guid? resourceId, DateTime? startTime, DateTime? endtime, ReservationStatus? status)
        {
            var reservations = await _reservationRepository.GetAllAsync(resourceId, startTime, endtime, status);

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

        private static void ValidateReservation(Reservation reservation, Resource resource)
        {
            if (resource.Status == ResourceStatus.Unavailable) throw new InvalidOperationException("Resource is Unavailable");

            if (!resource.Weekends && (reservation.StartDate.DayOfWeek == DayOfWeek.Saturday || reservation.StartDate.DayOfWeek == DayOfWeek.Sunday
            || reservation.EndDate.DayOfWeek == DayOfWeek.Saturday || reservation.EndDate.DayOfWeek == DayOfWeek.Sunday))
                throw new InvalidOperationException("It's not allowed reservations on weekends");

            if (TimeOnly.FromDateTime(reservation.StartDate) < resource.OpeningTime || TimeOnly.FromDateTime(reservation.EndDate) > resource.ClosingTime)
                throw new InvalidOperationException("Reservation must be within Resource Hours");
        }

        private static void ValidateCapacityNotExceeded(Reservation reservation, Resource resource, List<Reservation> reservations)
        {
            int current = 0;
            int max = 0;
            var allReservations = reservations.Append(reservation);

            var events = allReservations.SelectMany(r => new[]
            {
                (time: r.StartDate, delta: r.NumberOfPeople),
                (time: r.EndDate, delta: -r.NumberOfPeople)
            })
            .OrderBy(e => e.time).ThenBy(e => e.delta);

            foreach (var e in events)
            {
                current += e.delta;
                max = Math.Max(max, current);
            }

            if (max > resource.Capacity) throw new InvalidOperationException(resource.Name + " has reached capacity limit.");
        }
    }
}