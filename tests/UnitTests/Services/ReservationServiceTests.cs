using Moq;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.Models;
using BookingSystem.Jobs.Interface;
using BookingSystem.ExceptionHelper;
using BookingSystem.UnitTests.Helpers;

namespace BookingSystem.UnitTests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJobScheduler> _mockJobScheduler;
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJobScheduler = new Mock<IJobScheduler>();
            _reservationService = new ReservationService(_mockReservationRepository.Object, _mockResourceRepository.Object, _mockUserRepository.Object, _mockJobScheduler.Object);
        }

        [Fact]
        public async Task CreateReservationAsync_WithValidDto_CreatesReservation()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.ReservationDTO(resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());
            _mockReservationRepository.Setup(repo => repo.AddAsync(It.IsAny<Reservation>()));

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);

            _mockReservationRepository.Verify(repo => repo.AddAsync(It.IsAny<Reservation>()), Times.Once);
            _mockJobScheduler.Verify(x => x.ScheduleReservationExpiration(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateReservationAsync_WithOutsideHours_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.OutsideHoursReservationDTO(resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Assert & Act
            var exception = await Assert.ThrowsAsync<DomainException>(() => _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString()));
            Assert.Equal("Reservation must be within Resource Hours", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_WithWeekendNotAllowed_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(false);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.WeekendReservationDTO(resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Assert & Act
            var exception = await Assert.ThrowsAsync<DomainException>(() => _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString()));
            Assert.Equal("It's not allowed reservations on weekends", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_WithCapacityOverflow_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.OverCapacityReservationDTO(resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Assert & Act
            var exception = await Assert.ThrowsAsync<DomainException>(() => _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString()));
            Assert.Equal($"{resource.Name} has reached capacity limit.", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_WithInactiveResource_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.ReservationDTO(resource.Id);

            resource.DeactivateResource();

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Assert & Act
            var exception = await Assert.ThrowsAsync<DomainException>(() => _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString()));
            Assert.Equal("Resource is Unavailable", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_AtOpeningTime_ShouldCreateReservation()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var user = CreateEntities.UserModel();
            var reservationDto = CreateEntities.ReservationDTO(resource.Id);

            reservationDto.StartDate = Utils.FutureDate(8);
            reservationDto.EndDate = reservationDto.StartDate.AddHours(1);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);
        }

        [Fact]
        public async Task CreateReservationAsync_AtClosingTime_ShouldThrowDomainException()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservationDto = CreateEntities.ReservationDTO(resource.Id);
            reservationDto.StartDate = Utils.FutureDate(18);
            reservationDto.EndDate = Utils.FutureDate(19);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservationDto.StartDate, reservationDto.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationDto, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);
        }

        [Fact]
        public async Task CancelReservationAsync_WithValidId_CancelsReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.CancelReservationAsync(reservation.Id, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelled", result.Status);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task ConfirmReservationAsync_WithValidId_ConfirmsReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.ConfirmReservationAsync(reservation.Id, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Confirmed", result.Status);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_WithValidData_UpdatesResource()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateResourceAsync(reservation.Id, resource.Id, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resource.Id, result.Resource.ResourceId);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_WithNonPendingReservation_ShouldThrowDomainException()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            reservation.ConfirmReservation();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DomainException>(() => _reservationService.UpdateResourceAsync(reservation.Id, resource.Id, user.Id.ToString()));
            Assert.Equal("Only pending reservations can change the number of people.", exception.Message);
        }

        [Fact]
        public async Task UpdateDateAsync_WithValidData_UpdatesDates()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            var newStartDate = Utils.FutureDate(14);
            var newEndDate = Utils.FutureDate(15);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(reservation.ResourceId, newStartDate, newEndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateDateAsync(reservation.Id, newStartDate, newEndDate, user.Id.ToString());

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
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            var newNumberOfPeople = 10;

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resource.Id, reservation.StartDate, reservation.EndDate, new[] { ReservationStatus.Confirmed, ReservationStatus.Pending }, user.Id)).ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.UpdateNumberOfPeopleAsync(reservation.Id, newNumberOfPeople, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newNumberOfPeople, result.NumberOfPeople);
            _mockReservationRepository.Verify(repo => repo.UpdateAsync(reservation), Times.Once);
        }

        [Fact]
        public async Task GetReservationsAsync_WithValidFilters_ReturnsReservations()
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

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockResourceRepository.Setup(repo => repo.GetByIdsAsync(resourceIds)).ReturnsAsync(resources);
            _mockUserRepository.Setup(repo => repo.GetByIdsAsync(usersIds)).ReturnsAsync(users);

            _mockReservationRepository.Setup(repo => repo.GetAllAsync(resources[0].Id, startTime, endTime, status, user.Id)).ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservationsAsync(resources[0].Id, startTime, endTime, status, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(resources[0].Id, result[0].Resource.ResourceId);
        }

        [Fact]
        public async Task GetReservationByIdAsync_WithValidId_ReturnsReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(user.Id.ToString(), resource.Id);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(reservation.ResourceId)).ReturnsAsync(resource);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.GetReservationByIdAsync(reservation.Id, user.Id.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation.Id, result.Id);
        }

        [Fact]
        public async Task DeleteReservationAsync_WithValidId_DeletesReservation()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var reservation = CreateEntities.Reservation(user.Id.ToString(), Guid.NewGuid());

            _mockReservationRepository.Setup(repo => repo.GetByIdAsync(reservation.Id, user.Id)).ReturnsAsync(reservation);
            _mockReservationRepository.Setup(repo => repo.DeleteAsync(reservation));

            // Act
            await _reservationService.DeleteReservationAsync(reservation.Id, user.Id.ToString());

            // Assert
            _mockReservationRepository.Verify(repo => repo.DeleteAsync(reservation), Times.Once);
        }
    }
}