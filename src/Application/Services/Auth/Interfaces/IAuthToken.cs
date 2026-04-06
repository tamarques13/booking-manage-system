using BookingSystem.Domain.Models;

namespace BookingSystem.Application.Services.Auth.Interfaces
{
    public interface IAuthToken
    {
        Task RevokeAllTokensForUser(Guid userId);
        Task ValidateRefreshTokenAsync(RefreshToken oldToken);
    }
}