using System;
using System.Collections.Generic;
using System.Linq;
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
        // ✅ PUBLIC: Accessible with public token (Booking Site) or user token (Admin)
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
        public async Task<IActionResult> PutProperty(int id, [FromBody] PropertyCreateDto dto)
        {
            // Validar que el ModelState sea válido
            if (!ModelState.IsValid)
            {
                Console.WriteLine($"❌ [PUT Property] ModelState Invalid:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"   Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(new { 
                    error = "Validation failed", 
                    details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() 
                });
            }

            if (id != dto.Id)
                return BadRequest(new { error = "ID mismatch", message = $"URL id ({id}) does not match body id ({dto.Id})" });

            try
            {
                var tenantId = GetTenantId();
                if (tenantId == null)
                    return Unauthorized();

                // Log para debugging
                Console.WriteLine($"✅ [PUT Property] ID: {id}, Name: {dto.Name}");
                Console.WriteLine($"   HouseRules: {(dto.HouseRules != null ? "Present" : "Null")}");
                if (dto.HouseRules != null)
                {
                    Console.WriteLine($"   HouseRules.ImportantInfo: {(dto.HouseRules.ImportantInfo != null ? $"Count={dto.HouseRules.ImportantInfo.Count}" : "null")}");
                    Console.WriteLine($"   HouseRules.CustomNotes: {dto.HouseRules.CustomNotes ?? "null"}");
                }

                var updated = await _propertyService.UpdateAsync(id, dto, tenantId.Value);
                if (!updated)
                    return NotFound(new { error = "Property not found or unauthorized" });
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PUT Property Error]: {ex.Message}");
                Console.WriteLine($"❌ [Stack]: {ex.StackTrace}");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message, details = ex.InnerException?.Message });
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