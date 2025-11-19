using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var payments = await _paymentService.GetAllAsync(tenantId.Value);
            return Ok(payments);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var payment = await _paymentService.GetByIdAsync(tenantId.Value, id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> PostPayment(PaymentCreateDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var result = await _paymentService.CreateAsync(dto, tenantId.Value);
            return CreatedAtAction(nameof(GetPayment), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPayment(int id, PaymentCreateDto dto)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var updated = await _paymentService.UpdateAsync(tenantId.Value, id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var deleted = await _paymentService.DeleteAsync(tenantId.Value, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}