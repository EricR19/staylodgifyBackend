using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<AvailabilityDto>> GetAllAsync(int tenantId);
        Task<AvailabilityDto?> GetByIdAsync(int tenantId, int id);
        Task<AvailabilityDto> CreateAsync(AvailabilityDto dto, int tenantId);
        Task<bool> UpdateAsync(int tenantId, int id, AvailabilityDto dto);
        Task<bool> DeleteAsync(int tenantId, int id);
    }
}