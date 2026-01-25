using BookingSystem.DTOs;
using BookingSystem.Models;
using BookingSystem.Repositories.Interfaces;

namespace BookingSystem.Helpers
{
    public static class ResourceMapper
    {
        public static ResourceDto ToResourceDto(this Resource resource)
        {
            return new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Capacity = resource.Capacity,
                Type = resource.Type.ToString(),
                Status = resource.Status.ToString()
            };
        }
    }
}
