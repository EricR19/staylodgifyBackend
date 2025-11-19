using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BookingSite.Infrastructure.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly BookingDbContext _context;

        public AvailabilityRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            return await _context.Availabilities
                .Include(a => a.Room)
                .ThenInclude(r => r.Property)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Availability>> GetAllAsync()
        {
            return await _context.Availabilities
                .Include(a => a.Room)
                .ThenInclude(r => r.Property)
                .ToListAsync();
        }

        public async Task<Availability?> GetByRoomIdAndDateAsync(int roomId, DateTime date)
        {
            return await _context.Availabilities
                .Include(a => a.Room)
                .ThenInclude(r => r.Property)
                .FirstOrDefaultAsync(a => a.room_id == roomId && a.Date == date.Date);
        }

        public async Task<IEnumerable<Availability>> GetByRoomIdAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            return await _context.Availabilities
                .Include(a => a.Room)
                .ThenInclude(r => r.Property)
                .Where(a => a.room_id == roomId && a.Date >= startDate.Date && a.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Availability availability)
        {
            await _context.Availabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Availability availability)
        {
            _context.Availabilities.Update(availability);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability != null)
            {
                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
        }
    }
}