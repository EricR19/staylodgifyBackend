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
            // Normalize the search name for comparison
            var normalizedSearch = name.ToLowerInvariant().Trim();
            
            // Also create a version with hyphens replaced by spaces for URL-friendly matching
            var searchWithSpaces = normalizedSearch.Replace("-", " ");
            
            return await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => 
                    t.Name.ToLower() == normalizedSearch ||
                    t.Name.ToLower() == searchWithSpaces ||
                    t.Name.ToLower().Replace(" ", "-") == normalizedSearch);
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