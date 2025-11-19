using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync(int tenantId);
        Task<PaymentDto?> GetByIdAsync(int tenantId, int id);
        Task<PaymentDto> CreateAsync(PaymentCreateDto dto, int tenantId);
        Task<bool> UpdateAsync(int tenantId, int id, PaymentCreateDto dto);
        Task<bool> DeleteAsync(int tenantId, int id);
    }
}