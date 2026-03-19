using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserTokenDto> CreateUser(CreateUserDto dto);
        Task<UserTokenDto> LoginUser(LoginUserDto dto);
    }
}