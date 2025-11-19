using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payments>, IPaymentRepository
    {
        private readonly BookingDbContext _context;

        public PaymentRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payments>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.Tenant_Id == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payments>> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.Reservation_Id == reservationId)
                .ToListAsync();
        }

        public async Task<Payments?> GetByIdAndTenantIdAsync(int id, int tenantId)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.Tenant_Id == tenantId);
        }
    }
}