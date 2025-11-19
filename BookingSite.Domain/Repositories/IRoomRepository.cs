using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<IEnumerable<Room>> GetByPropertyIdAsync(int propertyId);
    }
}