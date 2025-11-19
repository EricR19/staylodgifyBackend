using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        private int? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
                return tenantId;
            return null;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAvailabilities()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var availabilities = await _availabilityService.GetAllAsync(tenantId.Value);
            return Ok(availabilities);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AvailabilityDto>> GetAvailability(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var availability = await _availabilityService.GetByIdAsync(tenantId.Value, id);
            if (availability == null)
                return NotFound();

            return Ok(availability);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AvailabilityDto>> PostAvailability(AvailabilityDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var result = await _availabilityService.CreateAsync(dto, tenantId.Value);
            return CreatedAtAction(nameof(GetAvailability), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAvailability(int id, AvailabilityDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var updated = await _availabilityService.UpdateAsync(tenantId.Value, id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var deleted = await _availabilityService.DeleteAsync(tenantId.Value, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}