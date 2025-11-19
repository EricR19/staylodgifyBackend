using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync(int tenantId);
    Task<IEnumerable<RoomDto>> GetByPropertyIdAsync(int tenantId, int propertyId);
    Task<RoomDto?> GetByIdAsync(int tenantId, int id);
    Task<RoomDto> CreateAsync(RoomDto dto, int tenantId);
    Task<bool> UpdateAsync(int tenantId, int id, RoomDto dto);
    Task<bool> DeleteAsync(int tenantId, int id);
}