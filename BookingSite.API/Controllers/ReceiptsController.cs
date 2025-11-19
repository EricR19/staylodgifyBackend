using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingSite.Application.DTOs;
using BookingSite.Application.Services;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
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
        public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetReceipts()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var receipts = await _receiptService.GetAllByTenantAsync(tenantId.Value);
            return Ok(receipts);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReceiptDto>> GetReceipt(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var receipt = await _receiptService.GetByIdAsync(tenantId.Value, id);
            if (receipt == null)
                return NotFound();

            return Ok(receipt);
        }

        [HttpGet("by-reservation/{reservationId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetByReservation(int reservationId)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var receipts = await _receiptService.GetByReservationIdAsync(tenantId.Value, reservationId);
            return Ok(receipts);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReceiptDto>> PostReceipt(ReceiptDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var result = await _receiptService.CreateAsync(dto, tenantId.Value);
            return CreatedAtAction(nameof(GetReceipt), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutReceipt(int id, ReceiptDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var updated = await _receiptService.UpdateAsync(tenantId.Value, id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReceipt(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var deleted = await _receiptService.DeleteAsync(tenantId.Value, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}