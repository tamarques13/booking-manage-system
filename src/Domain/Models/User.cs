using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Models
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
        public ICollection<Reservation> Reservations { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public User(string email, string passwordHash, string firstName, string lastName, UserRoles role)
        {
            if (string.IsNullOrEmpty(email)) throw new DomainException("User must have an Email");
            if (string.IsNullOrEmpty(passwordHash)) throw new DomainException("User must have an Password");
            if (string.IsNullOrEmpty(firstName)) throw new DomainException("User must have an FirstName");
            if (string.IsNullOrEmpty(lastName)) throw new DomainException("User must have an LastName");

            Id = Guid.NewGuid();
            Email = email;
            Password = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }

        public void Update(string email, string passwordHash, string firstName, string lastName, UserRoles role)
        {
            if (string.IsNullOrEmpty(email)) throw new DomainException("User must have an Email");
            if (string.IsNullOrEmpty(passwordHash)) throw new DomainException("User must have an Password");
            if (string.IsNullOrEmpty(firstName)) throw new DomainException("User must have an FirstName");
            if (string.IsNullOrEmpty(lastName)) throw new DomainException("User must have an LastName");

            Email = email;
            Password = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }
    }
}