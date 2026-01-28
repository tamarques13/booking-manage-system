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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(Guid id, CreateResourceDto dto)
        {
            var resourceDto = await _resourceService.UpdateResourceAsync(id, dto);
            return Ok(resourceDto);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateResource(Guid id)
        {
            var resourceDto = await _resourceService.ActivateResourceAsync(id);
            return Ok(resourceDto);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateResource(Guid id)
        {
            var resourceDto = await _resourceService.DeactivateResourceAsync(id);
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