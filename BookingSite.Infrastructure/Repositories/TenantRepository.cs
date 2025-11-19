using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        private readonly BookingDbContext _context;

        public TenantRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByNameAsync(string name)
        {
            return await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<IEnumerable<Tenant>> GetByStatusAsync(string status)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Status == status)
                .ToListAsync();
        }
    }
}