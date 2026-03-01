using System;
using BookingSystem.Models;
using Xunit;
using BookingSystem.ExceptionHelper;

namespace BookingSystem.UnitTests.Models
{
    public class ReservationModelTests
    {
        [Fact]
        public void Reservation_WithValidInputs_ShouldInitializeReservation()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(1);
            var endDate = DateTime.Now.AddHours(2);
            var numberOfPeople = 5;
            var resourceId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            var reservation = new Reservation(startDate, endDate, numberOfPeople, resourceId, userId);

            // Assert
            Assert.Equal(startDate, reservation.StartDate);
            Assert.Equal(endDate, reservation.EndDate);
            Assert.Equal(numberOfPeople, reservation.NumberOfPeople);
            Assert.Equal(resourceId, reservation.ResourceId);
            Assert.Equal(userId, reservation.UserId);
            Assert.Equal(ReservationStatus.Pending, reservation.Status);
        }

        [Fact]
        public void Reservation_WhenStartDateIsAfterEndDate_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(2);
            var endDate = DateTime.Now.AddHours(1);
            var numberOfPeople = 5;
            var resourceId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Reservation(startDate, endDate, numberOfPeople, resourceId, userId));
            Assert.Equal("EndDate must be after StartDate", exception.Message);
        }

        [Fact]
        public void Reservation_WhenStartDateIsBeforeCurrentTime_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(-1);
            var endDate = DateTime.Now.AddHours(1);
            var numberOfPeople = 5;
            var resourceId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Reservation(startDate, endDate, numberOfPeople, resourceId, userId));
            Assert.Equal("StartDate cannot be in the past", exception.Message);
        }

        [Fact]
        public void Reservation_WhenNumberOfPeopleIsNegative_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(1);
            var endDate = DateTime.Now.AddHours(2);
            var resourceId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            var exceptionZero = Assert.Throws<DomainException>(() => new Reservation(startDate, endDate, 0, resourceId, userId));
            Assert.Equal("NumberOfPeople must be greater than zero", exceptionZero.Message);
            var exceptionNegative = Assert.Throws<DomainException>(() => new Reservation(startDate, endDate, -1, resourceId, userId));
            Assert.Equal("NumberOfPeople must be greater than zero", exceptionZero.Message);
        }

        [Fact]
        public void ConfirmReservation_WhenCalled_ShouldSetStatusToConfirmed()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());

            // Act
            reservation.ConfirmReservation();

            // Assert
            Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        }

        [Fact]
        public void CancelReservation_WhenCalled_ShouldSetStatusToCancelled()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());

            // Act
            reservation.CancelReservation();

            // Assert
            Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        }

        [Fact]
        public void ExpireReservation_WhenCalled_ShouldSetStatusToExpired()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());

            // Act
            reservation.ExpireReservation();

            // Assert
            Assert.Equal(ReservationStatus.Expired, reservation.Status);
        }


        [Fact]
        public void UpdateDates_WhenCalled_ShouldUpdateStartAndEndDates()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());
            var newStartDate = DateTime.Now.AddHours(3);
            var newEndDate = DateTime.Now.AddHours(4);

            // Act
            reservation.UpdateDateReservation(newStartDate, newEndDate);

            // Assert
            Assert.Equal(newStartDate, reservation.StartDate);
            Assert.Equal(newEndDate, reservation.EndDate);
        }

        [Fact]
        public void UpdateDates_WhenStatusIsNotPeding_ShouldThrowDomainException()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());
            var newStartDate = DateTime.Now.AddHours(3);
            var newEndDate = DateTime.Now.AddHours(2);
            reservation.ConfirmReservation();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => reservation.UpdateDateReservation(newStartDate, newEndDate));
            Assert.Equal("Only pending reservations can be confirmed.", exception.Message);
        }

        [Fact]
        public void UpdateDates_WhenStartDateIsBeforeCurrentTime_ShouldThrowDomainException()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());
            var newStartDate = DateTime.Now.AddHours(-1);
            var newEndDate = DateTime.Now.AddHours(2);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => reservation.UpdateDateReservation(newStartDate, newEndDate));
            Assert.Equal("StartDate cannot be in the past", exception.Message);
        }

        [Fact]
        public void UpdateDates_WhenStartDateIsAfterEndDate_ShouldThrowDomainException()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());
            var newStartDate = DateTime.Now.AddHours(3);
            var newEndDate = DateTime.Now.AddHours(2);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => reservation.UpdateDateReservation(newStartDate, newEndDate));
            Assert.Equal("EndDate must be after StartDate", exception.Message);
        }

        [Fact]
        public void UpdateNumberOfPeople_WhenCalled_ShouldUpdateNumberOfPeople()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());
            var newNumberOfPeople = 10;

            // Act
            reservation.UpdateNumberOfPeople(newNumberOfPeople);

            // Assert
            Assert.Equal(newNumberOfPeople, reservation.NumberOfPeople);
        }

        [Fact]
        public void UpdateNumberOfPeople_ShouldThrowDomainException_WhenNumberOfPeopleIsZeroOrNegative()
        {
            // Arrange
            var reservation = new Reservation(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 5, Guid.NewGuid(), Guid.NewGuid());

            // Act & Assert
            var exceptionZero = Assert.Throws<DomainException>(() => reservation.UpdateNumberOfPeople(0));
            Assert.Equal("NumberOfPeople must be greater than zero", exceptionZero.Message);
            var exceptionNegative = Assert.Throws<DomainException>(() => reservation.UpdateNumberOfPeople(-1));
            Assert.Equal("NumberOfPeople must be greater than zero", exceptionNegative.Message);
        }
    }
}