using Moq;
using Xunit;
using BookingSystem.Services;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.DTOs;
using BookingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSystem.UnitTests
{
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly ResourceService _resourceService;

        public ResourceServiceTests()
        {
            _mockResourceRepository = new Mock<IResourceRepository>();
            _resourceService = new ResourceService(_mockResourceRepository.Object);
        }

        [Fact]
        public async Task CreateResource_WithValidDto_CallsRepositoryAddMethod()
        {
            // Arrange
            var resourceDto = new CreateResourceDto
            {
                Name = "New Resource",
                Capacity = 15,
                Type = "MeetingRoom",
                OpeningTime = new TimeOnly(9, 0),
                ClosingTime = new TimeOnly(17, 0),
                Weekends = true
            };

            var resource = new Resource("New Resource", 15, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockResourceRepository.Setup(repo => repo.AddAsync(It.IsAny<Resource>()));

            // Act
            var result = await _resourceService.CreateResourceAsync(resourceDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Resource", result.Name);
            _mockResourceRepository.Verify(repo => repo.AddAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_WithValidDto_UpdatesResource()
        {
            // Arrange
            var resourceId = Guid.NewGuid();
            var resource = new Resource("Old Resource", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);
            var updateDto = new CreateResourceDto
            {
                Name = "Updated Resource",
                Capacity = 20,
                Type = "ConferenceRoom",
                OpeningTime = new TimeOnly(8, 0),
                ClosingTime = new TimeOnly(18, 0),
                Weekends = false
            };

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.UpdateResourceAsync(resourceId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Resource", result.Name);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.Once);
        }

        [Fact]
        public async Task ActivateResourceAsync_WithValidId_ActivatesResource()
        {
            // Arrange
            var resourceId = Guid.NewGuid();
            var resource = new Resource("Resource", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), false);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            await _resourceService.DeactivateResourceAsync(resourceId);

            var result = await _resourceService.ActivateResourceAsync(resourceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Available", result.Status);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.AtLeast(2));
        }

        [Fact]
        public async Task DeactivateResourceAsync_WithValidId_DeactivatesResource()
        {
            // Arrange
            var resourceId = Guid.NewGuid();
            var resource = new Resource("Resource", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.DeactivateResourceAsync(resourceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unavailable", result.Status);
            _mockResourceRepository.Verify(repo => repo.UpdateAsync(resource), Times.Once);
        }

        [Fact]
        public async Task UpdateWeekendAsync_WithValidId_TogglesWeekendAvailability()
        {
            // Arrange
            var resourceId = Guid.NewGuid();
            var resource = new Resource("Resource", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.UpdateWeekendAsync(resourceId);

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
                new("Resource 1", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true),
                new("Resource 2", 20, ResourceType.ConferenceRoom, new TimeOnly(8, 0), new TimeOnly(18, 0), false)
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
            var resourceId = Guid.NewGuid();
            var resource = new Resource($"Resource {resourceId}", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            var result = await _resourceService.GetResourceByIdAsync(resourceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"Resource {resourceId}", result.Name);
            Assert.Equal(resource.Capacity, result.Capacity);
            _mockResourceRepository.Verify(repo => repo.GetByIdAsync(resourceId), Times.Once);
        }

        [Fact]
        public async Task DeleteResourceAsync_WithValidId_DeletesResource()
        {
            // Arrange
            var resourceId = Guid.NewGuid();
            var resource = new Resource("Resource", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0), true);

            _mockResourceRepository.Setup(repo => repo.GetByIdAsync(resourceId)).ReturnsAsync(resource);

            // Act
            await _resourceService.DeleteResourceAsync(resourceId);

            // Assert
            _mockResourceRepository.Verify(repo => repo.DeleteAsync(resource), Times.Once);
        }
    }
}