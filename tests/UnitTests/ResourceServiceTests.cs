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
        public async Task GetAllResources_WhenCalled_ReturnsListOfResources()
        {
            // Arrange
            var resources = new List<Resource>
            {
                new("Resource 1", 10, ResourceType.Office, new TimeOnly(9, 0), new TimeOnly(17, 0), true),
                new("Resource 2", 20, ResourceType.ConferenceRoom, new TimeOnly(8, 0), new TimeOnly(18, 0), false)
            };

            _mockResourceRepository.Setup(repo => repo.GetAllAsync())
                                   .ReturnsAsync(resources);

            // Act
            var result = await _resourceService.GetResourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Resource 1", result[0].Name);
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
    }
}