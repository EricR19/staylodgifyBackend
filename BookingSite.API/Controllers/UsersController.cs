using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSite.Application.DTOs;
using BookingSite.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        private int? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
                return tenantId;
            return null;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        // GET: api/Users/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            if (tenantId == null || userId == null)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(tenantId.Value, userId.Value);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            var users = await _userService.GetAllByTenantAsync(tenantId.Value);
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            if (tenantId == null || userId == null)
                return Unauthorized();

            if (id != userId && !User.IsInRole("admin"))
                return Forbid();

            var user = await _userService.GetByIdAsync(tenantId.Value, id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// ✅ SECURE LOGIN - Uses HttpOnly cookies, no token in response
        /// Kept for backward compatibility - delegates to AuthService
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                if (result == null || !result.Success)
                    return Unauthorized(new { success = false, error = result?.Error ?? "Invalid credentials" });

                // ✅ SECURE: Set JWT as HttpOnly cookie - token NOT exposed to JavaScript
                var isDevelopment = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
                
                Response.Cookies.Append("jwt", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !isDevelopment,
                    SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(1),
                    Path = "/"
                });

                // ✅ SECURE: Return user info but NOT the token
                return Ok(new { 
                    success = true,
                    user = new { 
                        id = result.User.Id,
                        name = result.User.Name, 
                        email = result.User.Email,
                        role = result.User.Role,
                        tenantId = result.User.TenantId
                    },
                    tenant = result.Tenant
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, error = "Authentication service error" });
            }
        }

        /// <summary>
        /// ✅ SECURE LOGOUT - Clears HttpOnly cookie
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var isDevelopment = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
            
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        // POST: api/Users
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] UserCreateDto dto)
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserDto dto)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            if (tenantId == null || userId == null)
                return Unauthorized();

            if (id != userId && !User.IsInRole("admin"))
                return Forbid();

            var updated = await _userService.UpdateAsync(tenantId.Value, id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            if (tenantId == null || userId == null)
                return Unauthorized();

            if (id != userId && !User.IsInRole("admin"))
                return Forbid();

            var deleted = await _userService.DeleteAsync(tenantId.Value, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // PUT: api/Users/{id}/change-password
        [HttpPut("{id}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            if (tenantId == null || userId == null)
                return Unauthorized();

            // ✅ SECURITY FIX: Users can ONLY change their own password
            if (id != userId)
                return Forbid("You can only change your own password");

            // Validate password confirmation
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("New password and confirmation do not match");

            // Validate password strength (add your own rules)
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                return BadRequest("Password must be at least 6 characters long");

            var result = await _userService.ChangePasswordAsync(tenantId.Value, id, dto);
            if (!result)
                return BadRequest("Current password is incorrect or user not found");

            return Ok(new { message = "Password changed successfully" });
        }

        // ✅ NEW: Admin Password Reset (generates token for manual distribution)
        [HttpPost("{id}/reset-password")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResetUserPassword(int id)
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
                return Unauthorized();

            // Generate secure reset token
            var resetToken = await _userService.InitiatePasswordResetAsync(tenantId.Value, id);
            if (resetToken == null)
                return NotFound("User not found");

            // Return token for manual distribution (since no email yet)
            return Ok(new { 
                message = "Password reset token generated successfully",
                resetToken = resetToken,
                expiresIn = "24 hours",
                instructions = "Provide this token to the user to reset their password"
            });
        }

        // ✅ NEW: User Self-Service Password Reset Request
        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            var result = await _userService.RequestPasswordResetByEmailAsync(dto.Email);
            
            // Always return success to prevent email enumeration attacks
            return Ok(new { 
                message = "If the email exists, a password reset token has been generated",
                instructions = "Check with your administrator for the reset token"
            });
        }

        // ✅ NEW: Reset Password with Token
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // Validate password confirmation
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("New password and confirmation do not match");

            // Validate password strength
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                return BadRequest("Password must be at least 6 characters long");

            var result = await _userService.ResetPasswordAsync(dto.Token, dto);
            if (!result)
                return BadRequest("Invalid or expired reset token");

            return Ok(new { message = "Password reset successfully" });
        }
    }
}