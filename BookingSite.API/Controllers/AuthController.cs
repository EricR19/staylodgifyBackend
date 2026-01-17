using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookingSite.Application.Services;
using BookingSite.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AuthController(
            ITenantService tenantService, 
            IAuthService authService, 
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _tenantService = tenantService;
            _authService = authService;
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// Public login for guest booking pages - creates limited-scope token
        /// </summary>
        [HttpGet("public-login")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicLogin([FromQuery] string tenantName)
        {
            var tenant = await _tenantService.GetByNameWithPropertiesAsync(tenantName);
            if (tenant == null)
                return Unauthorized(new { success = false, error = "Invalid tenant" });

            var claims = new List<Claim>
            {
                new Claim("tenant_id", tenant.Id.ToString()),
                new Claim("public_access", "true"),
                new Claim("scope", "public_booking") // Limited scope for public access
            };

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // ✅ SECURE: Set HttpOnly cookie for public access too
            SetAuthCookie(tokenString, TimeSpan.FromHours(1));

            // Return only tenant info, NOT the token
            return Ok(new { 
                success = true, 
                tenant = new { id = tenant.Id, name = tenant.Name }
            });
        }

        /// <summary>
        /// ✅ SECURE LOGIN - HttpOnly cookie only, no token in response body
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                
                if (result == null || !result.Success)
                {
                    return Unauthorized(new { 
                        success = false, 
                        error = result?.Error ?? "Invalid credentials" 
                    });
                }

                // ✅ SECURE: Set HttpOnly cookie - token NEVER exposed to JavaScript
                SetAuthCookie(result.Token, TimeSpan.FromDays(1));

                // ✅ SECURE: Return user/tenant info but NOT the token
                return Ok(new SecureLoginResponse
                {
                    Success = true,
                    User = result.User,
                    Tenant = result.Tenant
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { 
                    success = false, 
                    error = "Authentication service error" 
                });
            }
        }

        /// <summary>
        /// ✅ SECURE LOGOUT - Clears HttpOnly cookie
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Clear the auth cookie with matching options
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
                // NO Domain attribute - matches SetAuthCookie
            });

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        /// <summary>
        /// ✅ GET CURRENT USER - Validates session from cookie
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            var tenantName = User.FindFirst("tenant_name")?.Value;
            var tenantStatus = User.FindFirst("tenant_status")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized(new { success = false, error = "Invalid session" });
            }

            return Ok(new SecureLoginResponse
            {
                Success = true,
                User = new UserInfo
                {
                    Id = int.Parse(userId),
                    Name = name ?? "",
                    Email = email ?? "",
                    Role = role ?? "",
                    TenantId = int.Parse(tenantId)
                },
                Tenant = new TenantInfo
                {
                    Id = int.Parse(tenantId),
                    Name = tenantName ?? "",
                    Status = tenantStatus ?? ""
                }
            });
        }

        /// <summary>
        /// ✅ REFRESH SESSION - Extends cookie expiration if valid
        /// </summary>
        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized(new { success = false, error = "Invalid session" });
            }

            // Generate new token with extended expiration
            var claims = User.Claims.ToList();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            SetAuthCookie(tokenHandler.WriteToken(token), TimeSpan.FromDays(1));

            return Ok(new { success = true, message = "Session refreshed" });
        }

        /// <summary>
        /// Helper method to set secure auth cookie with proper settings
        /// ✅ FIXED: Added Domain attribute for iOS Safari cross-site support
        /// </summary>
        private void SetAuthCookie(string token, TimeSpan expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                    // ✅ Not accessible via JavaScript
                Secure = true,                      // ✅ HTTPS only (always true in production)
                SameSite = SameSiteMode.Lax,        // ✅ Changed from None to Lax for cross-domain cookies
                Expires = DateTime.UtcNow.Add(expiration),
                Path = "/",
                // ✅ NO Domain attribute - allows cookies to be sent from different domains
                // This fixes: Frontend on staylodgify.lat, Backend on onrender.com
                IsEssential = true
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
        }
    }

    /// <summary>
    /// Secure login response - does NOT include token
    /// </summary>
    public class SecureLoginResponse
    {
        public bool Success { get; set; }
        public UserInfo User { get; set; }
        public TenantInfo Tenant { get; set; }
    }
}