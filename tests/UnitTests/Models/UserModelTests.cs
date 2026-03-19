using BookingSystem.Domain.Models;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.UnitTests.Models
{
    public class AuthModelTests
    {
        [Fact]
        public void User_WithValidInputs_ShouldInitializeUser()
        {
            // Arrange
            var email = "test@example.com";
            var passwordHash = "hashed_password";
            var firstName = "John";
            var lastName = "Doe";
            var role = UserRoles.User;

            // Act
            var user = new User(email, passwordHash, firstName, lastName, role);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
            Assert.Equal(passwordHash, user.Password);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(role, user.Role);
            Assert.NotEqual(Guid.Empty, user.Id);
        }

        [Fact]
        public void User_WithNoEmail_ShouldThrowDomainException()
        {
            // Arrange
            var email = "";
            var passwordHash = "hashed_password";
            var firstName = "John";
            var lastName = "Doe";
            var role = UserRoles.User;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new User(email, passwordHash, firstName, lastName, role));
            Assert.Equal("User must have an Email", exception.Message);
        }

        [Fact]
        public void User_WithNoPassword_ShouldThrowDomainException()
        {
            // Arrange
            var email = "test@example.com";
            var passwordHash = "";
            var firstName = "John";
            var lastName = "Doe";
            var role = UserRoles.User;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new User(email, passwordHash, firstName, lastName, role));
            Assert.Equal("User must have an Password", exception.Message);
        }

        [Fact]
        public void User_WithNoFirstName_ShouldThrowDomainException()
        {
            // Arrange
            var email = "test@example.com";
            var passwordHash = "hashed_password";
            var firstName = "";
            var lastName = "Doe";
            var role = UserRoles.User;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new User(email, passwordHash, firstName, lastName, role));
            Assert.Equal("User must have an FirstName", exception.Message);
        }

        [Fact]
        public void User_WithNoLastName_ShouldThrowDomainException()
        {
            // Arrange
            var email = "test@example.com";
            var passwordHash = "hashed_password";
            var firstName = "John";
            var lastName = "";
            var role = UserRoles.User;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new User(email, passwordHash, firstName, lastName, role));
            Assert.Equal("User must have an LastName", exception.Message);
        }
    }
}