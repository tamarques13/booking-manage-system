using BookingSystem.Domain.Models;
using BookingSystem.Domain.Exceptions;
using BookingSystem.UnitTests.Helpers;

namespace BookingSystem.UnitTests.Models
{
    public class ResourceModelTests
    {
        [Fact]
        public void Resource_WithValidInputs_ShouldInitializeResource()
        {
            // Arrange
            var name = "Conference Room";
            var capacity = 10;
            var type = ResourceType.ConferenceRoom;
            var openingTime = new TimeOnly(9, 0);
            var closingTime = new TimeOnly(17, 0);
            var weekends = true;

            // Act
            var resource = new Resource(name, capacity, type, openingTime, closingTime, weekends);

            // Assert
            Assert.NotNull(resource);
            Assert.Equal(name, resource.Name);
            Assert.Equal(capacity, resource.Capacity);
            Assert.Equal(type, resource.Type);
            Assert.Equal(openingTime, resource.OpeningTime);
            Assert.Equal(closingTime, resource.ClosingTime);
            Assert.Equal(ResourceStatus.Available, resource.Status);
            Assert.True(resource.Weekends);
        }

        [Fact]
        public void Resource_WithZeroClosingTime_ShouldThrowDomainException()
        {
            // Arrange
            var name = "Conference Room";
            var capacity = 10;
            var type = ResourceType.ConferenceRoom;
            var openingTime = new TimeOnly(9, 0);
            var closingTime = new TimeOnly(0, 0);
            var weekends = true;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Resource(name, capacity, type, openingTime, closingTime, weekends));
            Assert.Equal("Closing time cannot be 00:00:00.", exception.Message);
        }

        [Fact]
        public void Update_WithValidInputs_ShouldUpdateResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var newName = "Updated Room";
            var newCapacity = 20;
            var newType = ResourceType.ConferenceRoom;
            var newOpeningTime = new TimeOnly(8, 0);
            var newClosingTime = new TimeOnly(18, 0);

            // Act
            resource.Update(newName, newCapacity, newType, newOpeningTime, newClosingTime);

            // Assert
            Assert.Equal(newName, resource.Name);
            Assert.Equal(newCapacity, resource.Capacity);
            Assert.Equal(newType, resource.Type);
            Assert.Equal(newOpeningTime, resource.OpeningTime);
            Assert.Equal(newClosingTime, resource.ClosingTime);
        }

        [Fact]
        public void Update_WithZeroClosingTime_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            var name = "Conference Room";
            var capacity = 10;
            var type = ResourceType.ConferenceRoom;
            var openingTime = new TimeOnly(9, 0);
            var closingTime = new TimeOnly(0, 0);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => resource.Update(name, capacity, type, openingTime, closingTime));
            Assert.Equal("Closing time cannot be 00:00:00.", exception.Message);
        }

        [Fact]
        public void Update_WhenNameIsEmpty_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => resource.Update("", 10, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0)));
            Assert.Equal("Name cannot be empty.", exception.Message);
        }

        [Fact]
        public void Update_WhenCapacityIsNonPositive_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => resource.Update("Meeting Room", 0, ResourceType.MeetingRoom, new TimeOnly(9, 0), new TimeOnly(17, 0)));
            Assert.Equal("Capacity must be greater than zero.", exception.Message);
        }

        [Fact]
        public void Update_WhenResourceTypeIsInvalid_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => resource.Update("Meeting Room", 10, (ResourceType)999, new TimeOnly(9, 0), new TimeOnly(17, 0)));
            Assert.Equal("Invalid resource type: 999", exception.Message);
        }

        [Fact]
        public void DeactivateResource_WhenCalled_ShouldDeactivateResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act
            resource.DeactivateResource();

            // Assert
            Assert.Equal(ResourceStatus.Unavailable, resource.Status);
        }

        [Fact]
        public void ActivateResource_WhenCalled_ShouldActivateResource()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);
            resource.DeactivateResource();

            // Act
            resource.ActivateResource();

            // Assert
            Assert.Equal(ResourceStatus.Available, resource.Status);
        }

        [Fact]
        public void ActivateResource_WithAlreadyAvailable_ShouldThrowDomainException()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => resource.ActivateResource());
            Assert.Equal("Resource is already available.", exception.Message);
        }

        [Fact]
        public void UpdateWeekend_WhenCalled_ShouldUpdateWeekend()
        {
            // Arrange
            var resource = CreateEntities.Resource(true);

            // Act
            resource.UpdateWeekend();

            // Assert
            Assert.False(resource.Weekends);
        }
    }
}