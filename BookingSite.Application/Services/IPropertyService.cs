using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDto>> GetAllByTenantAsync(int tenantId);
        Task<PropertyDto?> GetByIdAndTenantAsync(int id, int tenantId);
        Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto dto, int tenantId);
        Task<bool> UpdateAsync(int id, PropertyCreateDto dto, int tenantId);
        Task<bool> DeleteAsync(int id, int tenantId);
    }
}