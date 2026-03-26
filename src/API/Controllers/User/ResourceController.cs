using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Application.Services.Interfaces;
using BookingSystem.API.Controllers.Base;

namespace BookingSystem.API.Controllers.User
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/resources")]
    [Authorize]
    [ApiController]
    public class ResourceController(IResourceService resourceService) : BaseController
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