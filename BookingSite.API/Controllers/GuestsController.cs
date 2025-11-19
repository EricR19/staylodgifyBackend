using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSite.Infrastructure.Context;
using BookingSite.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public GuestsController(BookingDbContext context)
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

        // GET: api/Guests
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var guests = await _context.Guests.Where(g => g.TenantId == tenantId).ToListAsync();
            return Ok(guests);
        }

        // GET: api/Guests/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id && g.TenantId == tenantId);
            if (guest == null)
            {
                return NotFound();
            }

            return Ok(guest);
        }

        // PUT: api/Guests/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutGuest(int id, Guest guest)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            if (id != guest.Id)
            {
                return BadRequest();
            }

            var existing = await _context.Guests.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id && g.TenantId == tenantId);
            if (existing == null)
                return NotFound();

            guest.TenantId = tenantId.Value; // Ensure tenant cannot be changed
            _context.Entry(guest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuestExists(id, tenantId.Value))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Guests
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guest>> PostGuest(Guest guest)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            guest.TenantId = tenantId.Value; // Always set from claims
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuest), new { id = guest.Id }, guest);
        }

        // DELETE: api/Guests/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id && g.TenantId == tenantId);
            if (guest == null)
            {
                return NotFound();
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuestExists(int id, int tenantId)
        {
            return _context.Guests.Any(e => e.Id == id && e.TenantId == tenantId);
        }
    }
}