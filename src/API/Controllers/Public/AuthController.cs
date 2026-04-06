using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Application.Services.Auth.Interfaces;
using BookingSystem.Application.DTOs;
using BookingSystem.API.Controllers.Base;

namespace BookingSystem.API.Controllers.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseController
    {
        private readonly IAuthService _authService = authService;

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var UserTokenDto = await _authService.CreateUserAsync(dto, IpAddress);
            return StatusCode(StatusCodes.Status201Created, UserTokenDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto dto)
        {
            var UserTokenDto = await _authService.LoginUserAsync(dto, IpAddress);
            return Ok(UserTokenDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string Token)
        {
            var UserTokenDto = await _authService.RotateRefreshTokenAsync(Token, IpAddress);
            return Ok(UserTokenDto);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogOutAsync(Guid.Parse(UserId));
            return NoContent();
        }
    }
}