using BookingSite.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllByTenantIdAsync(int tenantId);
        Task<Reservation?> GetByIdAndTenantIdAsync(int id, int tenantId);
        Task<IEnumerable<Reservation>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task DeleteAsync(int id);
    }
}