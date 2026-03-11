using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.DTOs;
using BookingSystem.Services.Interfaces;

namespace BookingSystem.Controllers.User
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/resources")]
    [Authorize]
    [ApiController]
    public class ResourceController(IResourceService resourceService) : ControllerBase
    {
        private readonly IResourceService _resourceService = resourceService;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceService.GetResourcesAsync();
            return Ok(resources);
        }
    }
}