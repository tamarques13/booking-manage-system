using BookingSystem.Application.DTOs;
using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Security;
using BookingSystem.Application.Mappers;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Application.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Updates an existing user with the provided information.
        /// The password is securely hashed before being stored.
        /// </summary>
        /// 
        /// <param name="userId">The Id of the user to update.</param>
        /// <param name="dto">The data transfer object containing the updated user information.</param>
        /// <returns>The updated user mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">Thrown when the user does not exist.</exception>
        /// <exception cref="DomainException">Thrown when the user violates business rules.</exception>

        public async Task<UserDto> UpdateUserByIdAsync(Guid userId, CreateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var hash = PasswordHasher.HashPassword(dto.Password);

            user.Update(dto.Email, hash, dto.FirstName, dto.LastName, Enum.Parse<UserRoles>(dto.Role));

            await _userRepository.UpdateByIdAsync(user);

            return user.ToUserDto();
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// 
        /// <param name="userId">The Id of the user to retrieve.</param>
        /// <returns>The user mapped to a DTO.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">Thrown when the user does not exist.</exception>

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            return user.ToUserDto();
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// 
        /// <returns>A list of users mapped to DTOs.</returns>
        
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var usersList = users.Select(u => u.ToUserDto()).ToList();

            return usersList;
        }


        /// <summary>
        /// Deletes a user from the system by their unique identifier. Should not be used if historical booking data must be preserved.
        /// </summary>
        /// 
        /// <param name="userId">The Id of the user to delete.</param>
        /// 
        /// <exception cref="KeyNotFoundException">Thrown when the user does not exist.</exception>
      
        public async Task DeleteUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            await _userRepository.DeleteByIdAsync(user);
        }
    }
}