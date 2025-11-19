using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant?> GetByNameAsync(string name);
        Task<IEnumerable<Tenant>> GetByStatusAsync(string status);
    }
}