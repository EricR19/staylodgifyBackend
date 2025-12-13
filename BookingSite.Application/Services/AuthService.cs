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
            // Debug: Log that user was not found
            Console.WriteLine($"[AUTH DEBUG] User not found for email: {loginDto.Email}");
            return new LoginResponseDto
            {
                Success = false,
                Error = "Invalid email or password"
            };
        }

        // Debug: Log user found
        Console.WriteLine($"[AUTH DEBUG] User found: ID={user.Id}, Email={user.email}, TenantID={user.Tenant_id}");
        Console.WriteLine($"[AUTH DEBUG] Password hash starts with: {user.password_hash?.Substring(0, Math.Min(20, user.password_hash?.Length ?? 0))}...");

        // ✅ HANDLE LEGACY PASSWORDS (Migration Support)
        bool passwordValid = false;
        bool needsHashMigration = false;

        try
        {
            // Try BCrypt verification first (for new passwords)
            passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.password_hash);
            Console.WriteLine($"[AUTH DEBUG] BCrypt.Verify result: {passwordValid}");
        }
        catch (BCrypt.Net.SaltParseException ex)
        {
            Console.WriteLine($"[AUTH DEBUG] BCrypt SaltParseException: {ex.Message}");
            // If BCrypt fails, check if it's a plain text password (legacy)
            if (user.password_hash == loginDto.Password)
            {
                passwordValid = true;
                needsHashMigration = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH DEBUG] BCrypt Exception: {ex.GetType().Name}: {ex.Message}");
        }

        if (!passwordValid)
        {
            Console.WriteLine($"[AUTH DEBUG] Password validation failed for user: {user.email}");
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

        // Debug tenant info
        Console.WriteLine($"[AUTH DEBUG] Tenant: Name={user.Tenant?.Name}, Status={user.Tenant?.Status}, Expires={user.Tenant?.Subscription_expires_at}");

        // ✅ TENANT STATUS VALIDATION - Prevent access to inactive tenants
        if (user.Tenant?.Status != "active")  // Fixed: Capital 'S'
        {
            Console.WriteLine($"[AUTH DEBUG] FAILED: Tenant status is '{user.Tenant?.Status}', expected 'active'");
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
            Console.WriteLine($"[AUTH DEBUG] FAILED: Subscription expired on {user.Tenant.Subscription_expires_at.Value}, today is {DateTime.Today}");
            return new LoginResponseDto
            {
                Success = false,
                Error = "Your organization's subscription has expired"
            };
        }

        Console.WriteLine($"[AUTH DEBUG] All checks passed, generating JWT token...");

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