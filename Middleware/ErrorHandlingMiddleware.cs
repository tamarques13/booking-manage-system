using Microsoft.AspNetCore.Mvc;
using BookingSystem.ExceptionHelper;

namespace BookingSystem.Middleware
{
    internal sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred");

                var statusCode = ex switch
                {
                    DomainException => StatusCodes.Status400BadRequest,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;

                var json = new ProblemDetails
                {
                    Title = "An error occured",
                    Status = statusCode,
                    Detail = statusCode == StatusCodes.Status500InternalServerError ? "An unexpected error occurred" : ex.Message,
                };

                await context.Response.WriteAsJsonAsync(json);
            }
        }
    }
}
