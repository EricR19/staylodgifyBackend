using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class LogRepository : Repository<Logs>, ILogRepository
    {
        private readonly BookingDbContext _context;

        public LogRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Logs>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Logs
                .AsNoTracking()
                .Where(l => l.Tenant_Id == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Logs>> GetByUserIdAsync(int userId)
        {
            return await _context.Logs
                .AsNoTracking()
                .Where(l => l.User_Id == userId)
                .ToListAsync();
        }
    }
}