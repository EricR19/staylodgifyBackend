using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetByTenantIdAsync(int tenantId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailWithTenantAsync(string email);
    }
}