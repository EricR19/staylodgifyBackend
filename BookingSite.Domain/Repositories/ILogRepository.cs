using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface ILogRepository : IRepository<Logs>
    {
        Task<IEnumerable<Logs>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<Logs>> GetByUserIdAsync(int userId);
    }
}