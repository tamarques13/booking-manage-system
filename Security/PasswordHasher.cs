namespace BookingSystem.Security
{
    /// <summary>
    /// Provides password hashing and verification utilities using BCrypt enhanced hashing.
    /// This implementation ensures secure one-way password storage.
    /// </summary>

    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes the provided plain text password using BCrypt enhanced hashing algorithm.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>A securely hashed password string.</returns>

        public static string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

            return passwordHash;
        }

        /// <summary>
        /// Verifies whether the provided plain text password matches the stored password hash.
        /// </summary>
        /// <param name="password">The plain text password entered by the user.</param>
        /// <param name="passwordHash">The stored hashed password to verify against.</param>
        /// <returns>True if the password is valid, otherwise false.</returns>

        public static bool VerifyPassword(string password, string passwordHash)
        {
            var result = BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);

            return result;
        }
    }
}