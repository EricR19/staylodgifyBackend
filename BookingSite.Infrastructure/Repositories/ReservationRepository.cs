using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly BookingDbContext _context;

        public ReservationRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllByTenantIdAsync(int tenantId)
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .AsNoTracking()
                .Where(r => r.Tenant_Id == tenantId)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAndTenantIdAsync(int id, int tenantId)
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.Tenant_Id == tenantId);
        }

        public async Task<IEnumerable<Reservation>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .AsNoTracking()
                .Where(r => r.Room_Id == roomId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId)
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .AsNoTracking()
                .Where(r => r.Guest_Id == guestId)
                .ToListAsync();
        }

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }
    }
}