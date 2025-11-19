using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSite.Infrastructure.Context;
using BookingSite.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public LogsController(BookingDbContext context)
        {
            _context = context;
        }

        private int? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
                return tenantId;
            return null;
        }

        // GET: api/Logs
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Logs>>> GetLogs()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var logs = await _context.Logs
                .Where(l => l.Tenant_Id == tenantId)
                .ToListAsync();

            return Ok(logs);
        }

        // GET: api/Logs/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Logs>> GetLog(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var log = await _context.Logs
                .FirstOrDefaultAsync(l => l.Id == id && l.Tenant_Id == tenantId);

            if (log == null)
                return NotFound();

            return Ok(log);
        }

        // POST: api/Logs
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Logs>> PostLog(Logs log)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            log.Tenant_Id = tenantId.Value; // Always set from claims
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLog), new { id = log.Id }, log);
        }

        // PUT: api/Logs/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutLog(int id, Logs log)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            if (id != log.Id)
                return BadRequest();

            var existing = await _context.Logs.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id && l.Tenant_Id == tenantId);

            if (existing == null)
                return NotFound();

            log.Tenant_Id = tenantId.Value; // Ensure tenant cannot be changed
            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogExists(id, tenantId.Value))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var log = await _context.Logs
                .FirstOrDefaultAsync(l => l.Id == id && l.Tenant_Id == tenantId);

            if (log == null)
                return NotFound();

            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogExists(int id, int tenantId)
        {
            return _context.Logs.Any(e => e.Id == id && e.Tenant_Id == tenantId);
        }
    }
}