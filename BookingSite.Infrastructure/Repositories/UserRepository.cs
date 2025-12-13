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
        // This includes the tenant for validation in the service layer
        public async Task<User?> GetByEmailWithTenantAsync(string email)
        {
            // Simplified query - tenant validation done in AuthService
            // This allows better error messages for expired/inactive tenants
            return await _context.Users
                .Include(u => u.Tenant)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == email);
        }
    }
}