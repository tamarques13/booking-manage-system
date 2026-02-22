namespace BookingSystem.ExceptionHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class
    /// with a specified error message describing the business rule violation.
    /// </summary>
    /// <param name="message">The error message explaining why the exception was thrown.</param>
    public class DomainException(string message) : Exception(message) { }
}