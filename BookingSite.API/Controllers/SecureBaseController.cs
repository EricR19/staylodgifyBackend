using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSite.API.Controllers
{
    /// <summary>
    /// ✅ SECURE BASE CONTROLLER
    /// Provides centralized, secure access to tenant and user context.
    /// All controllers should inherit from this for consistent security.
    /// </summary>
    [ApiController]
    public abstract class SecureBaseController : ControllerBase
    {
        /// <summary>
        /// Gets the tenant ID from the JWT claims.
        /// This is the ONLY trusted source for tenant identification.
        /// ⚠️ NEVER trust query params or headers for tenant!
        /// </summary>
        protected int? TenantId
        {
            get
            {
                // First try from middleware-set HttpContext.Items (fastest)
                if (HttpContext.Items.TryGetValue("TenantId", out var tenantIdFromMiddleware))
                {
                    return tenantIdFromMiddleware as int?;
                }

                // Fallback to claims
                var tenantIdClaim = User.FindFirst("tenant_id")?.Value 
                    ?? User.FindFirst("tenantId")?.Value;
                    
                if (int.TryParse(tenantIdClaim, out var tenantId))
                    return tenantId;
                    
                return null;
            }
        }

        /// <summary>
        /// Gets the current user ID from the JWT claims.
        /// </summary>
        protected int? CurrentUserId
        {
            get
            {
                // First try from middleware-set HttpContext.Items
                if (HttpContext.Items.TryGetValue("UserId", out var userIdFromMiddleware))
                {
                    return userIdFromMiddleware as int?;
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                    return userId;
                    
                return null;
            }
        }

        /// <summary>
        /// Gets the current user's role from the JWT claims.
        /// </summary>
        protected string? CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value;

        /// <summary>
        /// Gets the current user's email from the JWT claims.
        /// </summary>
        protected string? CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

        /// <summary>
        /// Gets the current user's name from the JWT claims.
        /// </summary>
        protected string? CurrentUserName => User.FindFirst(ClaimTypes.Name)?.Value;

        /// <summary>
        /// Checks if the current user has admin role.
        /// </summary>
        protected bool IsAdmin => User.IsInRole("admin");

        /// <summary>
        /// Checks if this is a public access token (for guest booking pages).
        /// </summary>
        protected bool IsPublicAccess
        {
            get
            {
                var publicAccess = User.FindFirst("public_access")?.Value;
                return publicAccess == "true";
            }
        }

        /// <summary>
        /// Gets the tenant status from the JWT claims.
        /// </summary>
        protected string? TenantStatus => User.FindFirst("tenant_status")?.Value;

        /// <summary>
        /// Returns Unauthorized if tenant context is missing.
        /// Use this at the start of protected endpoints.
        /// </summary>
        protected ActionResult? ValidateTenantContext()
        {
            if (TenantId == null)
            {
                return Unauthorized(new { 
                    success = false, 
                    error = "Tenant context not found",
                    message = "Please log in again"
                });
            }
            return null;
        }

        /// <summary>
        /// Returns Unauthorized if user context is missing.
        /// </summary>
        protected ActionResult? ValidateUserContext()
        {
            if (CurrentUserId == null || TenantId == null)
            {
                return Unauthorized(new { 
                    success = false, 
                    error = "User context not found",
                    message = "Please log in again"
                });
            }
            return null;
        }

        /// <summary>
        /// Returns Forbid if user is not admin.
        /// </summary>
        protected ActionResult? RequireAdmin()
        {
            if (!IsAdmin)
            {
                return Forbid();
            }
            return null;
        }

        /// <summary>
        /// Returns Forbid if user is trying to access another user's resource.
        /// Admins can access any user's resources within their tenant.
        /// </summary>
        protected ActionResult? ValidateUserAccess(int requestedUserId)
        {
            if (requestedUserId != CurrentUserId && !IsAdmin)
            {
                return Forbid();
            }
            return null;
        }
    }
}

