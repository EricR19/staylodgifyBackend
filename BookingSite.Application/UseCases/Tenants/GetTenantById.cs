using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;

namespace BookingSite.Application.UseCases.Tenants
{
    public class GetTenantById
    {
        private readonly ITenantRepository _tenantRepository;

        public GetTenantById(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<Tenant?> ExecuteAsync(int id)
        {
            return await _tenantRepository.GetByIdAsync(id);
        }
    }
}