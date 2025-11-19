using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // Helper to extract tenantId from claims
        private int? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value ?? User.FindFirst("tenantId")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
                return tenantId;
            return null;
        }

        // GET: api/Properties
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties()
        {
            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                var properties = await _propertyService.GetAllByTenantAsync(tenantId.Value);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Properties/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PropertyDto>> GetProperty(int id)
        {
            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                var property = await _propertyService.GetByIdAndTenantAsync(id, tenantId.Value);
                if (property == null)
                    return NotFound();
                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Properties/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProperty(int id, PropertyCreateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                var updated = await _propertyService.UpdateAsync(id, dto, tenantId.Value);
                if (!updated)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Properties
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PropertyDto>> PostProperty(PropertyCreateDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                var property = await _propertyService.CreatePropertyAsync(dto, tenantId.Value);
                return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Properties/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                var deleted = await _propertyService.DeleteAsync(id, tenantId.Value);
                if (!deleted)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}