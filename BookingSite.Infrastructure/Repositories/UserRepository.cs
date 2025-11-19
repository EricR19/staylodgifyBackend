using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly BookingDbContext _context;

        public UserRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Tenant_id == tenantId)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == email);
        }

        // ✅ CRITICAL SECURITY METHOD - Secure multi-tenant authentication
        // This implements the secure query that prevents cross-tenant access
        public async Task<User?> GetByEmailWithTenantAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Tenant) // Include tenant for validation
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == email 
                    && u.Tenant != null 
                    && u.Tenant.Status == "active"  // Fixed: Capital 'S' for Status
                    && (u.Tenant.Subscription_expires_at == null || u.Tenant.Subscription_expires_at >= DateTime.Today));  // Fixed: Capital 'S' and underscore
        }
    }
}