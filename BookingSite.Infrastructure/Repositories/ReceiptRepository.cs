using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Infrastructure.Repositories
{
    public class ReceiptRepository : Repository<Receipts>, IReceiptRepository
    {
        private readonly BookingDbContext _context;

        public ReceiptRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Receipts>> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Receipts
                .AsNoTracking()
                .Where(r => r.Reservation_Id == reservationId)
                .ToListAsync();
        }
    }
}