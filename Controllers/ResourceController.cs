using Microsoft.AspNetCore.Mvc;
using BookingSystem.DTOs;
using BookingSystem.Services;
using BookingSystem.Services.Interfaces;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourceController(IResourceService resourceService) : ControllerBase
    {
        private readonly IResourceService _resourceService = resourceService;

        [HttpPost]
        public async Task<IActionResult> CreateResource(CreateResourceDto dto)
        {
            var resourceDto = await _resourceService.CreateResourceAsync(dto);
            return Ok(resourceDto);
        }

        [HttpPut("{id}/book")]
        public async Task<IActionResult> BookResource(Guid id)
        {
            var resourceDto = await _resourceService.BookResourceAsync(id);
            return Ok(resourceDto);
        }

        [HttpPut("{id}/release")]
        public async Task<IActionResult> ReleaseResource(Guid id)
        {
            var resourceDto = await _resourceService.ReleaseResourceAsync(id);
            return Ok(resourceDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceService.GetResourcesAsync();
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResourceById(Guid id)
        {
            var resourceDto = await _resourceService.GetResourceByIdAsync(id);
            return Ok(resourceDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            await _resourceService.DeleteResourceAsync(id);
            return NoContent();
        }
    }
}