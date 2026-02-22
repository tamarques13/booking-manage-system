using BCrypt;

namespace BookingSystem.Security
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

            return passwordHash;
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            var result = BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);

            return result;
        }
    }
}