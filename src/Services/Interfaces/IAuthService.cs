using BookingSystem.DTOs;

namespace BookingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserTokenDto> CreateUser(CreateUserDto dto);
        Task<UserTokenDto> LoginUser(LoginUserDto dto);
    }
}