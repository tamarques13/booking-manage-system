using BookingSystem.ExceptionHelper;

namespace BookingSystem.Models
{
    public enum UserRoles { Admin, User }
    public class User
    {
        public User() { }
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRoles Role { get; set; }

        public User(string email, string passwordHash, string firstName, string lastName, UserRoles role)
        {
            Id = Guid.NewGuid();
            Email = email;
            Password = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }
    }
}