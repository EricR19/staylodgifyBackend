using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IGuestRepository : IRepository<Guest>
    {
        Task<IEnumerable<Guest>> GetAllAsync();
        Task<Guest?> GetByIdAsync(int id);
        Task<Guest?> GetByEmailAsync(string email);
        Task<Guest?> GetByEmailAndTenantIdAsync(string email, int tenantId);
        Task AddAsync(Guest guest);
        Task UpdateAsync(Guest guest);
        Task DeleteAsync(int id);
    }
}