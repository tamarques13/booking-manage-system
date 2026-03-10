using Moq;
using Xunit;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.UnitTests.Helpers;
using BookingSystem.Models;

namespace BookingSystem.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task UpdateUserByIdAsync_WithValidDto_UpdatesUserData()
        {
            // Arrange
            var user = CreateEntities.UserModel();
            var newUser = CreateEntities.CreateUserDto();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateUserByIdAsync(user.Id, newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test name", result.Name);
            _mockUserRepository.Verify(repo => repo.UpdateByIdAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var user = CreateEntities.UserModel();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User name", result.Name);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task GetUsersAsync_WhenCalled_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User>
            {
                CreateEntities.UserModel(),
                CreateEntities.UserModel()
            };

            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockUserRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteResourceAsync_WithValidId_DeletesResource()
        {
            // Arrange
            var user = CreateEntities.UserModel();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            await _userService.DeleteUserByIdAsync(user.Id);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteByIdAsync(user), Times.Once);
        }
    }
}