using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IPaymentRepository : IRepository<Payments>
    {
        Task<IEnumerable<Payments>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<Payments>> GetByReservationIdAsync(int reservationId);
        Task<Payments?> GetByIdAndTenantIdAsync(int id, int tenantId);
    }
}