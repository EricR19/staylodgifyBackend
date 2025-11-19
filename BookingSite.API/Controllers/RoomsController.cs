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
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
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
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var rooms = await _roomService.GetAllAsync(tenantId.Value);
            return Ok(rooms);
        }

        [HttpGet("by-property/{propertyId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByProperty(int propertyId)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var rooms = await _roomService.GetByPropertyIdAsync(tenantId.Value, propertyId);
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var room = await _roomService.GetByIdAsync(tenantId.Value, id);
            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RoomDto>> PostRoom(RoomDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            try
            {
                var result = await _roomService.CreateAsync(dto, tenantId.Value);
                return CreatedAtAction(nameof(GetRoom), new { id = result.Id }, result);
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRoom(int id, RoomDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            if (id != dto.Id)
                return BadRequest();

            var updated = await _roomService.UpdateAsync(tenantId.Value, id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var deleted = await _roomService.DeleteAsync(tenantId.Value, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}