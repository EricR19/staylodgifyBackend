using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;

namespace BookingSite.Application.UseCases.Tenants
{
    public class CreateTenant
    {
        private readonly ITenantRepository _tenantRepository;

        public CreateTenant(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task ExecuteAsync(Tenant tenant)
        {
            // Aquí puedes agregar validaciones de negocio antes de guardar
            await _tenantRepository.AddAsync(tenant);
        }
    }
}