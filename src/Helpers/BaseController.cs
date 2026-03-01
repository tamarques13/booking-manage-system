using Microsoft.AspNetCore.Mvc;
namespace BookingSystem.Helpers
{
    public abstract class BaseController : ControllerBase
    {
        protected string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found.");
    }
}