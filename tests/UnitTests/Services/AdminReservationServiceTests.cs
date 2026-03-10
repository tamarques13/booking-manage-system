using Xunit;
using Moq;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.Jobs.Interface;
using BookingSystem.UnitTests.Helpers;
using BookingSystem.Models;

namespace BookingSystem.UnitTests.Services
{
    public class AdminReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJobScheduler> _mockJobScheduler;
        private readonly ReservationService _reservationService;

        public AdminReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJobScheduler = new Mock<IJobScheduler>();
            _reservationService = new ReservationService(_mockReservationRepository.Object, _mockResourceRepository.Object, _mockUserRepository.Object, _mockJobScheduler.Object);
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
            var result = await _reservationService.CreateAdminReservationAsync(reservationDto);

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

            _mockReservationRepository.Setup(repo => repo.GetAdminByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateAdminReservationAsync(reservationDto, reservation.Id);

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

            _mockReservationRepository.Setup(repo => repo.GetAdminAllAsync(resources[0].Id, startTime, endTime, status, user.Id)).ReturnsAsync(reservations);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockResourceRepository.Setup(repo => repo.GetByIdsAsync(resourceIds)).ReturnsAsync(resources);
            _mockUserRepository.Setup(repo => repo.GetByIdsAsync(usersIds)).ReturnsAsync(users);

            // Act
            var result = await _reservationService.GetAdminReservationsAsync(resources[0].Id, startTime, endTime, status, user.Id.ToString());

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

            _mockReservationRepository.Setup(repo => repo.GetAdminByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _reservationService.GetAdminReservationByIdAsync(reservation.Id);

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

            _mockReservationRepository.Setup(repo => repo.GetAdminByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.DeleteAsync(reservation));

            // Act
            await _reservationService.DeleteAdminReservationByIdAsync(reservation.Id);

            // Assert
            _mockReservationRepository.Verify(repo => repo.DeleteAsync(reservation), Times.Once);
        }
    }
}