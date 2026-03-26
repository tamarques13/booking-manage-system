using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found.");

        protected string IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }
}