using Moq;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services.Auth;
using BookingSystem.Domain.Exceptions;
using BookingSystem.UnitTests.Helpers;
using BookingSystem.Application.Services.Auth.Interfaces;

namespace BookingSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly Mock<IAuthToken> _mockAuthToken;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly AuthService _authService;


        public AuthServiceTests()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockAuthToken = new Mock<IAuthToken>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _authService = new AuthService(
                _mockAuthToken.Object,
                _mockAuthRepository.Object,
                _mockRefreshTokenRepository.Object);

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
            var result = await _authService.CreateUserAsync(user, "0.0.0.0");

            // Assert
            _mockAuthRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            Assert.NotNull(result);
            Assert.StartsWith("Bearer ", result.AccessToken);
        }

        [Fact]
        public async Task CreateUser_WhenEmailIsAlreadyRegistered_ShouldThrowDomainException()
        {
            // Arrange
            var user = CreateEntities.RegisterUserDto();

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _authService.CreateUserAsync(user, "0.0.0.0"));
        }

        [Fact]
        public async Task LoginUser_WhenCredentialsAreValid_ShouldReturnToken()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();
            var user = CreateEntities.User(dto);

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginUserAsync(dto, "0.0.0.0");

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("Bearer ", result.AccessToken);
        }

        [Fact]
        public async Task LoginUser_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.LoginUserAsync(dto, "0.0.0.0"));
        }

        [Fact]
        public async Task LoginUser_WhenPasswordIsInvalid_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var dto = CreateEntities.LoginUserDto();
            var user = CreateEntities.WrongUser(dto);

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUserAsync(dto, "0.0.0.0"));
        }
    }
}