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
            // Normalize the search name - replace hyphens with spaces for URL-friendly matching
            var normalizedSearch = name.Trim();
            var searchWithSpaces = normalizedSearch.Replace("-", " ");
            
            // First try exact match (case-insensitive in MySQL by default with utf8 collation)
            var tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == normalizedSearch);
            
            if (tenant != null) return tenant;
            
            // Try with hyphens replaced by spaces
            tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == searchWithSpaces);
            
            if (tenant != null) return tenant;
            
            // Try case-insensitive search using LIKE
            tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => EF.Functions.Like(t.Name, searchWithSpaces));
            
            return tenant;
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