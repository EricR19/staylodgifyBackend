using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface ITenantService
    {
        Task<TenantDto?> GetByIdAsync(int tenantId);
        Task<TenantWithPropertiesDto?> GetWithPropertiesAsync(int tenantId);
        Task<TenantWithPropertiesDto?> GetByNameWithPropertiesAsync(string tenantName);
        Task<TenantDto> CreateAsync(TenantDto dto);
        Task<bool> UpdateAsync(int tenantId, TenantDto dto);
        Task<bool> DeleteAsync(int tenantId);
    }
}