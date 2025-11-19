using BookingSite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Domain.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<Availability?> GetByIdAsync(int id);
        Task<IEnumerable<Availability>> GetAllAsync();
        Task<Availability?> GetByRoomIdAndDateAsync(int roomId, DateTime date);
        Task<IEnumerable<Availability>> GetByRoomIdAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate);
        Task AddAsync(Availability availability);
        Task UpdateAsync(Availability availability);
        Task DeleteAsync(int id);
    }
}