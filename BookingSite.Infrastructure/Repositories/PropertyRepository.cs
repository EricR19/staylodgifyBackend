using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        private readonly BookingDbContext _context;

        public PropertyRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Properties
                .AsNoTracking()
                .Where(p => p.Tenant_Id == tenantId)
                .ToListAsync();
        }
    }
}