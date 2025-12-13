using System.Security.Claims;

namespace BookingSite.API.Middleware
{
    /// <summary>
    /// ✅ CRITICAL SECURITY MIDDLEWARE
    /// Validates tenant membership on every authenticated request.
    /// Prevents users from accessing data from other tenants.
    /// </summary>
    public class TenantValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantValidationMiddleware> _logger;

        public TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip validation for anonymous endpoints
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            // Extract tenant_id from JWT claims
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantStatus = context.User.FindFirst("tenant_status")?.Value;

            // ✅ SECURITY: Validate tenant_id exists in token
            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                _logger.LogWarning("Request rejected: No tenant_id in JWT. User: {UserId}", userIdClaim);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "Invalid session", 
                    message = "Tenant context not found" 
                });
                return;
            }

            // ✅ SECURITY: Reject requests for suspended tenants
            if (tenantStatus == "suspended" || tenantStatus == "inactive")
            {
                _logger.LogWarning("Request rejected: Tenant suspended. TenantId: {TenantId}, User: {UserId}", 
                    tenantIdClaim, userIdClaim);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "Account suspended", 
                    message = "Your organization's account has been suspended" 
                });
                return;
            }

            // ✅ SECURITY: Prevent tenant_id from being overridden via query params or headers
            // This is a common attack vector in multi-tenant systems
            if (context.Request.Query.ContainsKey("tenant") || 
                context.Request.Query.ContainsKey("tenantId") ||
                context.Request.Headers.ContainsKey("X-Tenant-Id"))
            {
                var requestedTenantId = context.Request.Query["tenant"].FirstOrDefault() 
                    ?? context.Request.Query["tenantId"].FirstOrDefault()
                    ?? context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

                if (requestedTenantId != null && requestedTenantId != tenantIdClaim)
                {
                    _logger.LogWarning(
                        "SECURITY ALERT: Tenant bypass attempt! JWT TenantId: {JwtTenantId}, Requested: {RequestedTenantId}, User: {UserId}",
                        tenantIdClaim, requestedTenantId, userIdClaim);
                    
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { 
                        error = "Access denied", 
                        message = "You cannot access resources from other organizations" 
                    });
                    return;
                }
            }

            // ✅ Store tenant_id in HttpContext.Items for easy access in controllers
            context.Items["TenantId"] = int.Parse(tenantIdClaim);
            context.Items["UserId"] = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim);

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method for registering the middleware
    /// </summary>
    public static class TenantValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantValidationMiddleware>();
        }
    }
}

