using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IReceiptService
    {
        Task<IEnumerable<ReceiptDto>> GetAllByTenantAsync(int tenantId);
        Task<ReceiptDto?> GetByIdAsync(int tenantId, int id);
        Task<IEnumerable<ReceiptDto>> GetByReservationIdAsync(int tenantId, int reservationId);
        Task<ReceiptDto> CreateAsync(ReceiptDto dto, int tenantId);
        Task<bool> UpdateAsync(int tenantId, int id, ReceiptDto dto);
        Task<bool> DeleteAsync(int tenantId, int id);
    }
}