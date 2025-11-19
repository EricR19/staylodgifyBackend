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

        public AuthController(ITenantService tenantService, IAuthService authService, IConfiguration configuration)
        {
            _tenantService = tenantService;
            _authService = authService;
            _configuration = configuration;
        }

        // GET: api/Auth/public-login?tenantName=amapolas
        [HttpGet("public-login")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicLogin([FromQuery] string tenantName)
        {
            // Usa tu método para obtener el tenant por nombre
            var tenant = await _tenantService.GetByNameWithPropertiesAsync(tenantName);
            if (tenant == null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim("tenant_id", tenant.Id.ToString()),
                new Claim("public_access", "true")
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

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        // ✅ CRITICAL SECURE LOGIN ENDPOINT - Fixed multi-tenant authentication
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

                // Set secure HTTP-only cookie for web clients
                if (result.Success)
                {
                    Response.Cookies.Append("jwt", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // Set to true in production
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(1)
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    error = "Authentication service error" 
                });
            }
        }
    }
}