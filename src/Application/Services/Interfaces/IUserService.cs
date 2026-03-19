using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> UpdateUserByIdAsync(Guid userId, CreateUserDto dto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<List<UserDto>> GetUsersAsync();
        Task DeleteUserByIdAsync(Guid userId);
    }
}