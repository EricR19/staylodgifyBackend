using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPropertyRepository _propertyRepository;

        public TenantService(ITenantRepository tenantRepository, IPropertyRepository propertyRepository)
        {
            _tenantRepository = tenantRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<TenantDto?> GetByIdAsync(int tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null) return null;

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactEmail = tenant.Contact_email,
                Plan = tenant.Plan,
                Status = tenant.Status,
                SubscriptionExpiresAt = tenant.Subscription_expires_at,
                CreatedAt = tenant.Created_at
            };
        }

        public async Task<TenantWithPropertiesDto?> GetWithPropertiesAsync(int tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null) return null;

            var properties = await _propertyRepository.GetByTenantIdAsync(tenantId);

            return new TenantWithPropertiesDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactEmail = tenant.Contact_email,
                Plan = tenant.Plan,
                Status = tenant.Status,
                SubscriptionExpiresAt = tenant.Subscription_expires_at,
                CreatedAt = tenant.Created_at,
                Properties = properties.Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task<TenantWithPropertiesDto?> GetByNameWithPropertiesAsync(string tenantName)
        {
            var tenant = await _tenantRepository.GetByNameAsync(tenantName);
            if (tenant == null) return null;

            var properties = await _propertyRepository.GetByTenantIdAsync(tenant.Id);

            return new TenantWithPropertiesDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactEmail = tenant.Contact_email,
                Plan = tenant.Plan,
                Status = tenant.Status,
                SubscriptionExpiresAt = tenant.Subscription_expires_at,
                CreatedAt = tenant.Created_at,
                Properties = properties.Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task<TenantDto> CreateAsync(TenantDto dto)
        {
            var tenant = new Tenant
            {
                Name = dto.Name,
                Contact_email = dto.ContactEmail,
                Plan = dto.Plan,
                Status = dto.Status,
                Subscription_expires_at = dto.SubscriptionExpiresAt,
                Created_at = System.DateTime.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);

            dto.Id = tenant.Id;
            dto.CreatedAt = tenant.Created_at;
            return dto;
        }

        public async Task<bool> UpdateAsync(int tenantId, TenantDto dto)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null) return false;

            tenant.Name = dto.Name;
            tenant.Contact_email = dto.ContactEmail;
            tenant.Plan = dto.Plan;
            tenant.Status = dto.Status;
            tenant.Subscription_expires_at = dto.SubscriptionExpiresAt;

            await _tenantRepository.UpdateAsync(tenant);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null) return false;

            await _tenantRepository.DeleteAsync(tenantId);
            return true;
        }
    }
}