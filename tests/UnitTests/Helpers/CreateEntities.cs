using BookingSystem.Domain.Models;
using BookingSystem.Application.DTOs;
using BookingSystem.Infrastructure.Security;

namespace BookingSystem.UnitTests.Helpers
{
    public class CreateEntities
    {
        public static Resource Resource(bool allowWeekends, int capacity = 10, ResourceType type = ResourceType.Office, int startHour = 8, int endHour = 19)
        {
            return new Resource(
                "Test Resource",
                capacity,
                type,
                new TimeOnly(startHour, 0),
                new TimeOnly(endHour, 0),
                allowWeekends);
        }
        public static CreateResourceDto ResourceDto(bool allowWeekends, int capacity = 10, string type = "Office", int startHour = 8, int endHour = 19)
        {
            return new CreateResourceDto
            {
                Name = "New Resource",
                Capacity = capacity,
                Type = type,
                OpeningTime = new TimeOnly(startHour, 0),
                ClosingTime = new TimeOnly(endHour, 0),
                Weekends = allowWeekends
            };
        }

        public static Reservation Reservation(string userId, Guid? resourceId, int startHour = 12, int endHour = 13, int people = 1)
        {
            return new Reservation(
                Utils.FutureDate(startHour),
                Utils.FutureDate(endHour),
                people,
                resourceId ?? Guid.NewGuid(),
                Guid.Parse(userId));
        }

        public static CreateReservationDto ReservationDTO(Guid? resourceId, int startHour = 12, int endHour = 13, int people = 1)
        {
            return new CreateReservationDto
            {
                NumberOfPeople = people,
                StartDate = Utils.FutureDate(startHour),
                EndDate = Utils.FutureDate(endHour),
                ResourceId = resourceId ?? Guid.Empty
            };
        }

        public static CreateReservationDto WeekendReservationDTO(Guid? resourceId, int people = 1)
        {
            return new CreateReservationDto
            {
                NumberOfPeople = people,
                StartDate = Utils.FutureSaturday(),
                EndDate = Utils.FutureSaturday().AddDays(1),
                ResourceId = resourceId ?? Guid.Empty
            };
        }

        public static CreateReservationDto OutsideHoursReservationDTO(Guid? resourceId, int startHour = 1, int endHour = 23, int people = 1)
        {
            return new CreateReservationDto
            {
                NumberOfPeople = people,
                StartDate = Utils.FutureDate(startHour),
                EndDate = Utils.FutureDate(endHour),
                ResourceId = resourceId ?? Guid.Empty
            };
        }

        public static CreateReservationDto OverCapacityReservationDTO(Guid? resourceId, int startHour = 12, int endHour = 13, int people = 1000)
        {
            return new CreateReservationDto
            {
                NumberOfPeople = people,
                StartDate = Utils.FutureDate(startHour),
                EndDate = Utils.FutureDate(endHour),
                ResourceId = resourceId ?? Guid.Empty
            };
        }

        public static CreateAdminReservationDto CreateAdminReservationDto(Guid userId, Guid? resourceId, int people = 1, int startHour = 12, int endHour = 13)
        {
            return new CreateAdminReservationDto
            {
                NumberOfPeople = people,
                StartDate = Utils.FutureDate(startHour),
                EndDate = Utils.FutureDate(endHour),
                ResourceId = resourceId ?? Guid.Empty,
                UserId = userId
            };
        }

        public static UpdateAdminReservationDto UpdateAdminReservationDto(Guid userId, Guid? resourceId, ReservationStatus status = ReservationStatus.Cancelled, int people = 1, int startHour = 12, int endHour = 13)
        {
            return new UpdateAdminReservationDto
            {
                Status = status,
                NumberOfPeople = people,
                StartDate = Utils.FutureDate(startHour),
                EndDate = Utils.FutureDate(endHour),
                ResourceId = resourceId ?? Guid.Empty,
                UserId = userId
            };
        }

        public static CreateUserDto RegisterUserDto(string email = "test@example.com", string password = "password123", string firstname = "John", string lastname = "Doe", string role = "User")
        {
            return new CreateUserDto
            {
                Email = email,
                Password = password,
                FirstName = firstname,
                LastName = lastname,
                Role = role
            };
        }

        public static User User(LoginUserDto dto)
        {
            return new User
            {
                Email = dto.Email,
                Password = PasswordHasher.HashPassword(dto.Password)
            };
        }

        public static User WrongUser(LoginUserDto dto)
        {
            return new User
            {
                Email = dto.Email,
                Password = PasswordHasher.HashPassword("WrongPassword")
            };
        }

        public static LoginUserDto LoginUserDto(string email = "test@example.com", string password = "password123")
        {
            return new LoginUserDto
            {
                Email = email,
                Password = password
            };
        }

        public static User UserModel(string email = "test@email.com", string password = "secret", string firstName = "User", string lastName = "name", UserRoles role = UserRoles.User)
        {
            return new User
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            };
        }

        public static CreateUserDto CreateUserDto(string email = "test@email.com", string password = "secret", string firstName = "test", string lastName = "name", string role = "User")
        {
            return new CreateUserDto
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            };
        }
    }
}