using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingSite.Application.DTOs;
using BookingSite.Application.Services;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        private int? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
                return tenantId;
            return null;
        }

        // GET: api/Tenants/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<TenantDto>> GetMyTenant()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var tenant = await _tenantService.GetByIdAsync(tenantId.Value);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        // GET: api/Tenants/me/properties
        [HttpGet("me/properties")]
        [Authorize]
        public async Task<ActionResult<TenantWithPropertiesDto>> GetMyTenantWithProperties()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var tenant = await _tenantService.GetWithPropertiesAsync(tenantId.Value);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        // GET: api/Tenants/by-name/{tenantName}
        [HttpGet("by-name/{tenantName}")]
        [AllowAnonymous]
        public async Task<ActionResult<TenantWithPropertiesDto>> GetTenantByName(string tenantName)
        {
            var tenant = await _tenantService.GetByNameWithPropertiesAsync(tenantName);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        // PUT: api/Tenants/me
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> PutMyTenant([FromBody] TenantDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            if (dto.Id != tenantId)
                return Forbid();

            var updated = await _tenantService.UpdateAsync(tenantId.Value, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Tenants/me
        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMyTenant()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var deleted = await _tenantService.DeleteAsync(tenantId.Value);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // POST: api/Tenants
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TenantDto>> PostTenant([FromBody] TenantDto dto)
        {
            var result = await _tenantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetMyTenant), new { }, result);
        }
    }
}