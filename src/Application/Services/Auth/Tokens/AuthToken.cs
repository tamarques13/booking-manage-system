using BookingSystem.Infrastructure.Persistence.Repositories.Interfaces;
using BookingSystem.Application.Services.Auth.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Application.Services.Auth.Tokens
{
    /// <summary>
    /// Provides security operations for refresh tokens, including validation,
    /// revocation and detection of token reuse attacks. This service enforces
    /// refresh token integrity and ensures that compromised tokens cannot be reused.
    /// </summary>

    public class AuthToken(IRefreshTokenRepository refreshTokenRepository) : IAuthToken
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        /// <summary>
        /// Revokes all refresh tokens belonging to the specified user.
        /// This is typically used during logout or when a token reuse attack is detected.
        /// </summary>
        /// <param name="userId">The identifier of the user whose tokens will be revoked.</param>

        public async Task RevokeAllTokensForUser(Guid userId)
        {
            var tokens = await _refreshTokenRepository.GetAsync(userId);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _refreshTokenRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Validates a refresh token by checking expiration and detecting reuse.
        /// If the token is expired, it is revoked and an <see cref="UnauthorizedAccessException"/> is thrown.
        /// If the token has already been revoked and replaced, a token reuse attack is assumed,
        /// all user tokens are revoked and a <see cref="SecurityException"/> is thrown.
        /// </summary>
        /// <param name="oldToken">The refresh token to validate.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the refresh token has expired.</exception>
        /// <exception cref="SecurityException">Thrown when refresh token reuse is detected (token replay attack).</exception>
        
        public async Task ValidateRefreshTokenAsync(RefreshToken oldToken)
        {
            if (oldToken.ExpireDate <= DateTime.UtcNow)
            {
                oldToken.IsRevoked = true;
                oldToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(oldToken);

                throw new UnauthorizedAccessException("Refresh token expired. Please log in again.");
            }

            if (oldToken.IsRevoked && oldToken.ReplacedByToken != null)
            {
                await RevokeAllTokensForUser(oldToken.UserId);
                throw new SecurityException("Refresh token reuse detected. Please log in again.");
            }
        }
    }
}