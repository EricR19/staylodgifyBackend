using BookingSite.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int tenantId, int userId);
        Task<IEnumerable<UserDto>> GetAllByTenantAsync(int tenantId);
        Task<UserDto> CreateAsync(UserCreateDto dto);
        Task<bool> UpdateAsync(int tenantId, int userId, UserDto dto);
        Task<bool> DeleteAsync(int tenantId, int userId);
        Task<bool> ChangePasswordAsync(int tenantId, int userId, ChangePasswordDto dto);
        
        // ✅ Password Reset Methods
        Task<string?> InitiatePasswordResetAsync(int tenantId, int userId);
        Task<bool> ResetPasswordAsync(string token, ResetPasswordDto dto);
        Task<bool> RequestPasswordResetByEmailAsync(string email);
    }
}