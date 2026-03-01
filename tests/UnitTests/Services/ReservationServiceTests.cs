using Moq;
using Xunit;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.DTOs;
using BookingSystem.Models;
using System.Linq.Expressions;
using BookingSystem.Jobs.Interface;
using BookingSystem.Jobs;

namespace BookingSystem.UnitTests
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IJobScheduler> _mockJobScheduler;
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockJobScheduler = new Mock<IJobScheduler>();
            _reservationService = new ReservationService(_mockReservationRepository.Object, _mockResourceRepository.Object, _mockJobScheduler.Object);
        }

        [Fact]
        public async Task CreateReservationAsync_WithValidDto_CreatesReservation()
        {
            // Arrange
            var year = DateTime.Now.Year + 2;

            var userId = Guid.NewGuid().ToString();
            var resource = new Resource("Meeting Room", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);
            var reservationDto = new CreateReservationDto
            {
                StartDate = new DateTime(year, 3, 5, 12, 0, 0),
                EndDate = new DateTime(year, 3, 5, 13, 0, 0),
                NumberOfPeople = 5,
                ResourceId = resource.Id
            };

            var reservation = new Reservation(reservationDto.StartDate, reservationDto.EndDate, reservationDto.NumberOfPeople, resource.Id, Guid.Parse(userId));

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, Guid.Parse(userId))).ReturnsAsync(new List<Reservation>());
            _mockReservationRepository.Setup(repo => repo.AddAsync(It.IsAny<Reservation>()));

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);
            _mockReservationRepository.Verify(repo => repo.AddAsync(It.IsAny<Reservation>()), Times.Once);
            _mockJobScheduler.Verify(x => x.ScheduleReservationExpiration(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CancelReservationAsync_WithValidId_CancelsReservation()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();

            var resource = new Resource("Meeting Room", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, resourceId, Guid.Parse(userId));

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _reservationService.CancelReservationAsync(reservationId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelled", result.Status);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task ConfirmReservationAsync_WithValidId_ConfirmsReservation()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();

            var resource = new Resource("Meeting Room", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, resourceId, Guid.Parse(userId));

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _reservationService.ConfirmReservationAsync(reservationId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Confirmed", result.Status);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_WithValidData_UpdatesResource()
        {
            // Arrange
            var year = DateTime.Now.Year + 2;

            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();
            var newResourceId = Guid.NewGuid();

            var reservation = new Reservation(new DateTime(year, 3, 5, 12, 0, 0), new DateTime(year, 3, 5, 13, 0, 0), 5, Guid.NewGuid(), Guid.Parse(userId));
            var newResource = new Resource("Conference Room", 20, ResourceType.ConferenceRoom, new TimeOnly(8, 0), new TimeOnly(18, 0), true);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(newResource.Id)).ReturnsAsync(newResource);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(newResource.Id, reservation.StartDate, reservation.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, Guid.Parse(userId))).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateResourceAsync(reservationId, newResource.Id, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newResource.Id, result.Resource.ResourceId);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task UpdateDateAsync_WithValidData_UpdatesDates()
        {
            // Arrange
            var year = DateTime.Now.Year + 2;

            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();
            var newStartDate = new DateTime(year, 3, 5, 11, 0, 0);
            var newEndDate = new DateTime(year, 3, 5, 12, 0, 0);

            var reservation = new Reservation(new DateTime(year, 3, 5, 14, 0, 0), new DateTime(year, 3, 5, 15, 0, 0), 5, Guid.NewGuid(), Guid.Parse(userId));
            var resource = new Resource("Conference Room", 20, ResourceType.ConferenceRoom, new TimeOnly(8, 0), new TimeOnly(18, 0), true);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, newStartDate, newEndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, Guid.Parse(userId))).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateDateAsync(reservationId, newStartDate, newEndDate, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newStartDate, result.StartDate);
            Assert.Equal(newEndDate, result.EndDate);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task UpdateNumberOfPeopleAsync_WithValidData_UpdatesNumberOfPeople()
        {
            // Arrange
            var year = DateTime.Now.Year + 2;

            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();
            var newNumberOfPeople = 10;

            var reservation = new Reservation(new DateTime(year, 3, 5, 12, 0, 0), new DateTime(year, 3, 5, 13, 0, 0), 5, Guid.NewGuid(), Guid.Parse(userId));
            var resource = new Resource("Conference Room", 20, ResourceType.ConferenceRoom, new TimeOnly(8, 0), new TimeOnly(18, 0), true);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, Guid.Parse(userId))).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateNumberOfPeopleAsync(reservationId, newNumberOfPeople, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newNumberOfPeople, result.NumberOfPeople);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task GetReservationsAsync_WithValidFilters_ReturnsReservations()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var status = new[] { ReservationStatus.Confirmed };

            var resources = new List<Resource> { new("Resource 1", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true) };
            var reservations = new List<Reservation> { new(startTime, endTime, 5, resources[0].Id, Guid.Parse(userId)) };

            var resourceIds = reservations.Select(r => r.ResourceId).Distinct().ToList();

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resources[0].Id, startTime, endTime, status, Guid.Parse(userId))).ReturnsAsync(reservations);
            _mockResourceRepository.Setup(repo => repo.GetByIdsAsync(resourceIds)).ReturnsAsync(resources);

            // Act
            var result = await _reservationService.GetReservationsAsync(resources[0].Id, startTime, endTime, status, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(resources[0].Id, result[0].Resource.ResourceId);
        }

        [Fact]
        public async Task GetReservationByIdAsync_WithValidId_ReturnsReservation()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();

            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.Parse(userId));
            var resource = new Resource("Resource 1", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);

            // Act
            var result = await _reservationService.GetReservationByIdAsync(reservationId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation.Id, result.Id);
        }

        [Fact]
        public async Task DeleteReservationAsync_WithValidId_DeletesReservation()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var reservationId = Guid.NewGuid();

            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.Parse(userId));

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservationId, Guid.Parse(userId))).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.DeleteAsync(reservation));

            // Act
            await _reservationService.DeleteReservationAsync(reservationId, userId);

            // Assert
            _mockReservationRepository.Verify(repo => repo.DeleteAsync(reservation), Times.Once);
        }
    }
}