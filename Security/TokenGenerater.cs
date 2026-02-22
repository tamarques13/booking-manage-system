using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookingSystem.Models;

namespace BookingSystem.Security
{
    public static class TokenGenerator
    {
        public static string GenerateBearerToken(User user)
        {
            var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY environment variable is not set.");
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("name", $"{user.FirstName} {user.LastName}"),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("ISSUER"),
                audience: Environment.GetEnvironmentVariable("AUDIENCE"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }
    }
}