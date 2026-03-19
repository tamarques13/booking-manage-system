using BookingSystem.Application.Services.Interfaces;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Security;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Application.Services
{
    /// <summary>
    /// Application service responsible for managing user authentication operations.
    /// Handles user lifecycle workflows and coordinates access to the underlying repository.
    /// </summary>

    public class AuthService(IAuthRepository authRepository) : IAuthService
    {
        private readonly IAuthRepository _authRepository = authRepository;

        /// <summary>
        /// Creates a new user account after validating that the email is not already registered.
        /// The password is securely hashed before storing the user information in the repository.
        /// </summary>
        /// <param name="dto">The data transfer object containing user registration details.</param>
        /// <returns>
        /// A <see cref="UserTokenDto"/> containing the generated bearer authentication token for the newly created user.
        /// </returns>
        /// <exception cref="DomainException">Thrown when the email is already registered.</exception>

        public async Task<UserTokenDto> CreateUser(CreateUserDto dto)
        {
            var existingUser = await _authRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) throw new DomainException("Email is already registered");

            var hash = PasswordHasher.HashPassword(dto.Password);
            var user = new User(dto.Email, hash, dto.FirstName, dto.LastName, Enum.Parse<UserRoles>(dto.Role));

            await _authRepository.AddAsync(user);

            return new UserTokenDto
            {
                Token = "Bearer " + TokenGenerator.GenerateBearerToken(user)
            };
        }

        /// <summary>
        /// Authenticates a user by verifying the provided password against the stored hashed password.
        /// If authentication is successful, a bearer token is generated and returned.
        /// </summary>
        /// <param name="dto">The data transfer object containing login credentials.</param>
        /// <returns>
        /// A <see cref="UserTokenDto"/> containing the generated bearer authentication token.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the password is invalid.</exception>

        public async Task<UserTokenDto> LoginUser(LoginUserDto dto)
        {
            var user = await _authRepository.GetByEmailAsync(dto.Email) ?? throw new KeyNotFoundException($"User with email: {dto.Email} not found.");

            var isValidPassword = PasswordHasher.VerifyPassword(dto.Password, user.Password);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Invalid email or password");

            return new UserTokenDto
            {
                Token = "Bearer " + TokenGenerator.GenerateBearerToken(user)
            };
        }
    }
}