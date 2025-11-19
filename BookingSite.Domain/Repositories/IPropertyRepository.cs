using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetByTenantIdAsync(int tenantId);
    }
}