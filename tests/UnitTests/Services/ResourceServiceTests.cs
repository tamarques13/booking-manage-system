using Moq;
using BookingSystem.Application.Services;
using BookingSystem.Infrastructure.Persistence.Repositories.Reservations.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.UnitTests.Helpers;
using BookingSystem.Domain.Exceptions;
using System.Linq.Expressions;

namespace BookingSystem.UnitTests.Services
{
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly ResourceService _resourceService;

        public ResourceServiceTests()
        {
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockReservationRepository = new Mock<IReservationRepository>();
            _resourceService = new ResourceService(_mockResourceRepository.Object, _mockReservationRepository.Object);
        }

        [Fact]
        public async Task CreateResource_WithValidDto_CallsRepositoryAddMethod()
        {
            // Arrange
            var resource = CreateEntities.ResourceDto(true);

            _mockResourceRepository.Setup(repo => repo.AddAsync(It.IsAny<Resource>()));

            // Act
            var result = await _resourceService.CreateResourceAsync(resource);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Resource", result.Name);
            _mockResourceRepository.Verify(repo => repo.AddAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_WithValidDto_UpdatesResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var newResource = CreateEntities.ResourceDto(false);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.UpdateResourceAsync(resource.Id, newResource);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Resource", result.Name);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.Once);
        }

        [Fact]
        public async Task ActivateResourceAsync_WithValidId_ActivatesResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            await _resourceService.DeactivateResourceAsync(resource.Id);

            var result = await _resourceService.ActivateResourceAsync(resource.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Available", result.Status);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.AtLeast(2));
        }

        [Fact]
        public async Task DeactivateResourceAsync_WithValidId_DeactivatesResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.DeactivateResourceAsync(resource.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unavailable", result.Status);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.Once);
        }

        [Fact]
        public async Task UpdateWeekendAsync_WithValidId_TogglesWeekendAvailability()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.UpdateWeekendAsync(resource.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Weekends);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.Once);
        }

        [Fact]
        public async Task GetResourcesAsync_WhenCalled_ReturnsAllResources()
        {
            // Arrange
            var resources = new List<Resource>
            {
                CreateEntities.Resource(true),
                CreateEntities.Resource(true),
            };

            _mockResourceRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(resources);

            // Act
            var result = await _resourceService.GetResourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockResourceRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetResourceByIdAsync_WithValidId_ReturnsResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.GetResourceByIdAsync(resource.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Resource", result.Name);
            Assert.Equal(resource.Capacity, result.Capacity);
            _mockResourceRepository.Verify(repo => repo.GetByIdAsync(resource.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteResourceAsync_WithValidId_DeletesResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);

            // Act
            await _resourceService.DeleteResourceAsync(resource.Id);

            // Assert
            _mockResourceRepository.Verify(repo => repo.DeleteAsync(resource), Times.Once);
        }

        [Fact]
        public async Task DeleteResource_WhenResourceHasReservations_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var reservation = CreateEntities.Reservation(Guid.NewGuid().ToString(), resource.Id);


            resource.Reservations.Add(reservation);
            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resource.Id)).ReturnsAsync(resource);
            _mockReservationRepository.Setup(repo => repo.AnyAsync(It.IsAny<Expression<Func<Reservation, bool>>>())).ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<DomainException>(() => _resourceService.DeleteResourceAsync(resource.Id));

            // Assert
            Assert.Equal("This item cannot be modified.", exception.Message);
        }
    }
}