using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Services.Interfaces;
using BookingSystem.DTOs;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var UserTokenDto = await _authService.CreateUser(dto);

            return StatusCode(StatusCodes.Status201Created, UserTokenDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto dto)
        {
            var UserTokenDto = await _authService.LoginUser(dto);
            return Ok(UserTokenDto);
        }
    }
}