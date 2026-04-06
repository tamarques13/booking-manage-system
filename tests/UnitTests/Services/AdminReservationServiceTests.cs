using Xunit;
using Moq;
using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services.Reservations.Interfaces;
using BookingSystem.Application.Services.Reservations;
using BookingSystem.Application.Jobs.Interface;
using BookingSystem.UnitTests.Helpers;
using BookingSystem.Domain.Models;

namespace BookingSystem.UnitTests.Services
{
    public class AdminReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IAdminReservationRepository> _mockAdminReservationRepository;
        private readonly Mock<IReservationCapacity> _mockReservationCapacity;
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJobScheduler> _mockJobScheduler;
        private readonly ReservationService _reservationService;
        private readonly AdminReservationService _adminReservationService;

        public AdminReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockAdminReservationRepository = new Mock<IAdminReservationRepository>();
            _mockReservationCapacity = new Mock<IReservationCapacity>();
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJobScheduler = new Mock<IJobScheduler>();
            _reservationService = new ReservationService(_mockReservationRepository.Object, _mockReservationCapacity.Object, _mockResourceRepository.Object, _mockUserRepository.Object, _mockJobScheduler.Object);
            _adminReservationService = new AdminReservationService(_mockReservationRepository.Object, _mockAdminReservationRepository.Object, _mockReservationCapacity.Object, _mockResourceRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task CreateAdminReservationAsync_WithValidDto_CreatesReservation()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.CreateAdminReservationDto(user.Id, resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());
            _mockReservationRepository.Setup(repo => repo.AddAsync(It.IsAny<Reservation>()));

            // Act
            var result = await _adminReservationService.CreateReservationAsync(reservationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);
            Assert.Equal("Confirmed", result.Status);

            _mockReservationRepository.Verify(repo => repo.AddAsync(It.IsAny<Reservation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAdminReservationAsync_WithValidId_UpdatesReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);
            var reservationDto = CreateEntities.UpdateAdminReservationDto(user.Id, resource.Id);

            _mockAdminReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _adminReservationService.UpdateReservationAsync(reservationDto, reservation.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelled", result.Status);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task GetAdminReservationsAsync_WithCalled_RetrievesAllReservations()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            var status = new[] { ReservationStatus.Confirmed };

            var resources = new List<Resource> { CreateEntities.Resource(true) };
            var users = new List<User> { CreateEntities.UserModel() };
            var reservations = new List<Reservation> { CreateEntities.Reservation(user.Id.ToString(), resources[0].Id) };

            var resourceIds = reservations.Select(r => r.ResourceId).Distinct().ToList();
            var usersIds = reservations.Select(r => r.UserId).Distinct().ToList();

            _mockAdminReservationRepository.Setup(repo => repo.GetAllAsync(resources[0].Id, startTime, endTime, status, user.Id)).ReturnsAsync(reservations);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockResourceRepository.Setup(repo => repo.GetByIdsAsync(resourceIds)).ReturnsAsync(resources);
            _mockUserRepository.Setup(repo => repo.GetByIdsAsync(usersIds)).ReturnsAsync(users);

            // Act
            var result = await _adminReservationService.GetReservationsAsync(resources[0].Id, startTime, endTime, status, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(resources[0].Id, result[0].Resource.ResourceId);
        }

        [Fact]
        public async Task GetAdminReservationByIdAsync_WithCalled_RetrievesReservationWithId()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            _mockAdminReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _adminReservationService.GetReservationByIdAsync(reservation.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation.Id, result.Id);
        }
        [Fact]
        public async Task DeleteAdminReservationByIdAsync_WithReservationId_DeletesReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var reservation = CreateEntities.Reservation(user.Id.ToString(), Guid.NewGuid());

            _mockAdminReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.DeleteAsync(reservation));

            // Act
            await _adminReservationService.DeleteReservationByIdAsync(reservation.Id);

            // Assert
            _mockReservationRepository.Verify(repo => repo.DeleteAsync(reservation), Times.Once);
        }
    }
}