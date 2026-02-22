using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.DTOs;
using BookingSystem.Services.Interfaces;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourceController(IResourceService resourceService) : ControllerBase
    {
        private readonly IResourceService _resourceService = resourceService;

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateResource(CreateResourceDto dto)
        {
            var resourceDto = await _resourceService.CreateResourceAsync(dto);
            return CreatedAtAction(nameof(GetResourceById), new { id = resourceDto.Id }, resourceDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(Guid id, CreateResourceDto dto)
        {
            var resourceDto = await _resourceService.UpdateResourceAsync(id, dto);
            return Ok(resourceDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateResource(Guid id)
        {
            var resourceDto = await _resourceService.ActivateResourceAsync(id);
            return Ok(resourceDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateResource(Guid id)
        {
            var resourceDto = await _resourceService.DeactivateResourceAsync(id);
            return Ok(resourceDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/weekends")]
        public async Task<IActionResult> UpdateWeekends(Guid id)
        {
            var resourceDto = await _resourceService.UpdateWeekendAsync(id);
            return Ok(resourceDto);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceService.GetResourcesAsync();
            return Ok(resources);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResourceById(Guid id)
        {
            var resourceDto = await _resourceService.GetResourceByIdAsync(id);
            return Ok(resourceDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            await _resourceService.DeleteResourceAsync(id);
            return NoContent();
        }
    }
}