using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSite.Application.DTOs;

namespace BookingSite.Application.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationCreateDto>> GetAllAsync(int tenantId);
        Task<ReservationCreateDto?> GetByIdAsync(int tenantId, int id);
        Task<ReservationCreateDto> CreateAsync(ReservationCreateRequestDto dto, int tenantId); // <-- tenantId added here
        Task<bool> UpdateAsync(int tenantId, int id, ReservationCreateDto dto);
        Task<bool> DeleteAsync(int tenantId, int id);
        Task<bool> ConfirmAsync(int tenantId, int reservationId);
        Task<bool> CancelAsync(int tenantId, int reservationId);
        
        Task<IEnumerable<ReservationCreateDto>> GetByRoomIdAsync(int roomId);
        
        Task<ReservationCreateDto> CreateWithProofAsync(ReservationWithProofRequestDto dto, int tenantId);
    }
}