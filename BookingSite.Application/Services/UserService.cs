using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net; // ✅ Fixed: Correct namespace

namespace BookingSite.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetByIdAsync(int tenantId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Tenant_id != tenantId)
                return null;

            return new UserDto
            {
                Id = user.Id,
                TenantId = user.Tenant_id,
                Name = user.name,
                Email = user.email,
                Role = user.Role,
                CreatedAt = user.created_at
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllByTenantAsync(int tenantId)
        {
            var users = await _userRepository.GetByTenantIdAsync(tenantId);
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                TenantId = user.Tenant_id,
                Name = user.name,
                Email = user.email,
                Role = user.Role,
                CreatedAt = user.created_at
            });
        }

        public async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            // ✅ SECURE PASSWORD HASHING using BCrypt
            var user = new User
            {
                Tenant_id = dto.TenantId,
                name = dto.Name,
                email = dto.Email,
                password_hash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // ✅ Fixed: Correct method call
                Role = dto.Role,
                created_at = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                TenantId = user.Tenant_id,
                Name = user.name,
                Email = user.email,
                Role = user.Role,
                CreatedAt = user.created_at
            };
        }

        public async Task<bool> UpdateAsync(int tenantId, int userId, UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Tenant_id != tenantId)
                return false;

            user.name = dto.Name;
            user.email = dto.Email;
            user.Role = dto.Role;
            // Do not update password here

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Tenant_id != tenantId)
                return false;

            await _userRepository.DeleteAsync(userId);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int tenantId, int userId, ChangePasswordDto dto)
        {
            // Validate passwords match
            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            // Get user and validate tenant ownership
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Tenant_id != tenantId)
                return false;

            // ✅ SECURE CURRENT PASSWORD VERIFICATION using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.password_hash)) // ✅ Fixed: Correct method call
                return false;

            // ✅ SECURE NEW PASSWORD HASHING using BCrypt
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword); // ✅ Fixed: Correct method call
            await _userRepository.UpdateAsync(user);
            
            return true;
        }

        // ✅ ADMIN INITIATED PASSWORD RESET
        public async Task<string?> InitiatePasswordResetAsync(int tenantId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Tenant_id != tenantId)
                return null;

            // Generate secure reset token
            var resetToken = Guid.NewGuid().ToString("N");
            
            // Set token and expiration (24 hours)
            user.reset_token = resetToken;
            user.reset_token_expires = DateTime.UtcNow.AddHours(24);
            
            await _userRepository.UpdateAsync(user);
            
            return resetToken;
        }

        // ✅ USER REQUESTED PASSWORD RESET BY EMAIL
        public async Task<bool> RequestPasswordResetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailWithTenantAsync(email);
            if (user == null || user.Tenant?.Status != "active")
                return false; // Don't reveal if user exists

            // Generate secure reset token
            var resetToken = Guid.NewGuid().ToString("N");
            
            // Set token and expiration (24 hours)
            user.reset_token = resetToken;
            user.reset_token_expires = DateTime.UtcNow.AddHours(24);
            
            await _userRepository.UpdateAsync(user);
            
            // TODO: Send email with reset link containing the token
            // Email would contain: https://yourdomain.com/reset-password?token={resetToken}
            
            return true;
        }

        // ✅ RESET PASSWORD WITH TOKEN
        public async Task<bool> ResetPasswordAsync(string token, ResetPasswordDto dto)
        {
            // Validate passwords match
            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            // Find user by reset token
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.reset_token == token 
                && u.reset_token_expires.HasValue 
                && u.reset_token_expires.Value > DateTime.UtcNow);

            if (user == null)
                return false; // Invalid or expired token

            // Validate tenant is active
            if (user.Tenant?.Status != "active")
                return false;

            // Update password and clear reset token
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword); // ✅ Fixed: Correct method call
            user.reset_token = null;
            user.reset_token_expires = null;
            
            await _userRepository.UpdateAsync(user);
            
            return true;
        }
    }
}