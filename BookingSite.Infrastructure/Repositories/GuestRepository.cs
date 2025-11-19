using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly BookingDbContext _context;

        public GuestRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guest>> GetAllAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<Guest?> GetByIdAsync(int id)
        {
            return await _context.Guests.FindAsync(id);
        }

        public async Task<Guest?> GetByEmailAsync(string email)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Email == email);
        }

        public async Task<Guest?> GetByEmailAndTenantIdAsync(string email, int tenantId)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Email == email && g.TenantId == tenantId);
        }

        public async Task AddAsync(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guest guest)
        {
            _context.Guests.Update(guest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }
        }
    }
}