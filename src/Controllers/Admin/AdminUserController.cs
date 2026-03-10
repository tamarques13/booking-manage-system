using BookingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.DTOs;

namespace BookingSystem.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById(Guid id, CreateUserDto dto)
        {
            var resourceDto = await _userService.UpdateUserByIdAsync(id, dto);
            return Ok(resourceDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateUserById(Guid id)
        {
            var resourceDto = await _userService.GetUserByIdAsync(id);
            return Ok(resourceDto);
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            var resourceDto = await _userService.GetUsersAsync();
            return Ok(resourceDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> GetUsers(Guid id)
        {
            await _userService.DeleteUserByIdAsync(id);
            return NoContent();
        }
    }
}