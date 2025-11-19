using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        private readonly BookingDbContext _context;

        public RoomRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Rooms
                .Include(r => r.Property)
                .Where(r => r.property_id == propertyId)
                .ToListAsync();
        }

        public override async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

    }
}