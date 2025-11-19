using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IReceiptRepository : IRepository<Receipts>
    {
        Task<IEnumerable<Receipts>> GetByReservationIdAsync(int reservationId);
    }
}