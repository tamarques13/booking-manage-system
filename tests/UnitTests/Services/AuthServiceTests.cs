using Moq;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services;
using BookingSystem.Domain.Exceptions;
using BookingSystem.UnitTests.Helpers;

namespace BookingSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _authService = new AuthService(_mockAuthRepository.Object);

            var config = TestConfiguration.Load();
            var secretKey = config["EnvironmentVariables:SECRET_KEY"];

            Environment.SetEnvironmentVariable("SECRET_KEY", secretKey);
        }

        [Fact]
        public async Task CreateUser_WithValidDto_ShouldCreateUser()
        {
            // Arrange
            var user = CreateEntities.RegisterUserDto();

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

            // Act
            var result = await _authService.CreateUser(user);

            // Assert
            _mockAuthRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            Assert.NotNull(result);
            Assert.StartsWith("Bearer ", result.Token);
        }

        [Fact]
        public async Task CreateUser_WhenEmailIsAlreadyRegistered_ShouldThrowDomainException()
        {
            // Arrange
            var user = CreateEntities.RegisterUserDto();

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _authService.CreateUser(user));
        }

        [Fact]
        public async Task LoginUser_WhenCredentialsAreValid_ShouldReturnToken()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();
            var user = CreateEntities.User(dto);

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginUser(dto);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("Bearer ", result.Token);
        }

        [Fact]
        public async Task LoginUser_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.LoginUser(dto));
        }

        [Fact]
        public async Task LoginUser_WhenPasswordIsInvalid_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();
            var user = CreateEntities.WrongUser(dto);

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUser(dto));
        }
    }
}