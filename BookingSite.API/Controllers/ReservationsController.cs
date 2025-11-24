using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    private int? GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (int.TryParse(tenantIdClaim, out var tenantId))
            return tenantId;
        return null;
    }

    // Endpoint para crear reservación pública (requiere JWT público)
    [HttpPost]
    [Authorize] // JWT público o privado
    public async Task<ActionResult<ReservationCreateDto>> PostReservation([FromBody] ReservationCreateRequestDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized(new { error = "Tenant ID not found in token" });

            var result = await _reservationService.CreateAsync(dto, tenantId.Value);
            return CreatedAtAction(nameof(GetReservation), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create reservation", message = ex.Message });
        }
    }

    // Endpoint para obtener fechas bloqueadas de un cuarto
    [HttpGet("blocked-dates/{roomId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetBlockedDates(int roomId)
    {
        var reservations = await _reservationService.GetByRoomIdAsync(roomId);
        var blockedDates = new List<string>();
        foreach (var r in reservations)
        {
            var date = r.StartDate.Date;
            while (date <= r.EndDate.Date)
            {
                blockedDates.Add(date.ToString("yyyy-MM-dd"));
                date = date.AddDays(1);
            }
        }
        return Ok(blockedDates);
    }
    
    [HttpPost("with-proof")]
    [Authorize]
    public async Task<ActionResult<ReservationCreateDto>> PostReservationWithProof([FromForm] ReservationWithProofRequestDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized(new { error = "Tenant ID not found in token" });

            var result = await _reservationService.CreateWithProofAsync(dto, tenantId.Value);
            return CreatedAtAction(nameof(GetReservation), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create reservation with proof", message = ex.Message });
        }
    }

    // (Opcional) Endpoint para obtener reservación por id
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ReservationCreateDto>> GetReservation(int id)
    {
        var tenantId = GetTenantId();
        if (tenantId == null)
            return Unauthorized();

        var reservation = await _reservationService.GetByIdAsync(tenantId.Value, id);
        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    // GET: api/Reservations
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReservationCreateDto>>> GetReservations()
    {
        var tenantId = GetTenantId();
        if (tenantId == null)
            return Unauthorized();

        var reservations = await _reservationService.GetAllAsync(tenantId.Value);
        return Ok(reservations);
    }

    // PUT: api/Reservations/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutReservation(int id, ReservationCreateDto dto)
    {
        var tenantId = GetTenantId();
        if (tenantId == null)
            return Unauthorized();

        if (id != dto.Id)
            return BadRequest();

        var updated = await _reservationService.UpdateAsync(tenantId.Value, id, dto);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/Reservations/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var tenantId = GetTenantId();
        if (tenantId == null)
            return Unauthorized();

        var deleted = await _reservationService.DeleteAsync(tenantId.Value, id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}