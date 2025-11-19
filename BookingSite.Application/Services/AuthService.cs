using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using BookingSite.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // ✅ Fixed: Correct namespace

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        // ✅ SECURE MULTI-TENANT AUTHENTICATION QUERY
        var user = await _userRepository.GetByEmailWithTenantAsync(loginDto.Email);
        
        if (user == null)
        {
            return new LoginResponseDto
            {
                Success = false,
                Error = "Invalid email or password"
            };
        }

        // ✅ HANDLE LEGACY PASSWORDS (Migration Support)
        bool passwordValid = false;
        bool needsHashMigration = false;

        try
        {
            // Try BCrypt verification first (for new passwords)
            passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.password_hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // If BCrypt fails, check if it's a plain text password (legacy)
            if (user.password_hash == loginDto.Password)
            {
                passwordValid = true;
                needsHashMigration = true;
            }
        }

        if (!passwordValid)
        {
            return new LoginResponseDto
            {
                Success = false,
                Error = "Invalid email or password"
            };
        }

        // ✅ MIGRATE LEGACY PASSWORD TO BCRYPT
        if (needsHashMigration)
        {
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
            await _userRepository.UpdateAsync(user);
        }

        // ✅ TENANT STATUS VALIDATION - Prevent access to inactive tenants
        if (user.Tenant?.Status != "active")  // Fixed: Capital 'S'
        {
            return new LoginResponseDto
            {
                Success = false,
                Error = "Your organization's account is suspended"
            };
        }

        // ✅ SUBSCRIPTION VALIDATION - Prevent access to expired tenants
        if (user.Tenant?.Subscription_expires_at.HasValue == true && 
            user.Tenant.Subscription_expires_at.Value < DateTime.Today)  // Fixed: Capital 'S' and underscore
        {
            return new LoginResponseDto
            {
                Success = false,
                Error = "Your organization's subscription has expired"
            };
        }

        // ✅ SECURE JWT TOKEN GENERATION with tenant context
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("tenant_id", user.Tenant_id.ToString()),
                new Claim("tenant_name", user.Tenant?.Name ?? ""),  // Fixed: Capital 'N'
                new Claim("tenant_status", user.Tenant?.Status ?? "")  // Fixed: Capital 'S'
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"])),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // ✅ SECURE RESPONSE with complete user and tenant information
        return new LoginResponseDto
        {
            Success = true,
            Token = tokenHandler.WriteToken(token),
            User = new UserInfo
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Role = user.Role,
                TenantId = user.Tenant_id
            },
            Tenant = new TenantInfo
            {
                Id = user.Tenant.Id,
                Name = user.Tenant.Name,  // Fixed: Capital 'N'
                Status = user.Tenant.Status,  // Fixed: Capital 'S'
                Plan = user.Tenant.Plan,  // Fixed: Capital 'P'
                SubscriptionExpiresAt = user.Tenant.Subscription_expires_at  // Fixed: Capital 'S' and underscore
            }
        };
    }
}