using System;
using System.Threading.Tasks;
using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Security;
using Xunit;
using Moq;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.ExceptionHelper;

namespace BookingSystem.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _authService = new AuthService(_mockAuthRepository.Object);

            Environment.SetEnvironmentVariable("SECRET_KEY", "super_secret_test_key_1234567890");
        }

        [Fact]
        public async Task CreateUser_WithValidDto_ShouldCreateUser()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Email = "test@example.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            };

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // Act
            var result = await _authService.CreateUser(dto);

            // Assert
            _mockAuthRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            Assert.NotNull(result);
            Assert.StartsWith("Bearer ", result.Token);
        }

        [Fact]
        public async Task CreateUser_WhenEmailIsAlreadyRegistered_ShouldThrowDomainException()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Email = "test@example.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            };

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _authService.CreateUser(dto));
        }

        [Fact]
        public async Task LoginUser_WhenCredentialsAreValid_ShouldReturnToken()
        {
            // Arrange
            var dto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Email = dto.Email,
                Password = PasswordHasher.HashPassword(dto.Password)
            };

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
            var dto = new LoginUserDto
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.LoginUser(dto));
        }

        [Fact]
        public async Task LoginUser_WhenPasswordIsInvalid_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var dto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Email = dto.Email,
                Password = PasswordHasher.HashPassword("correctpassword")
            };

            _mockAuthRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUser(dto));
        }
    }
}