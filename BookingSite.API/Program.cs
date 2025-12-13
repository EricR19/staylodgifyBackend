using BookingSite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using BookingSite.Domain.Repositories;
using BookingSite.Infrastructure.Repositories;
using BookingSite.API.Controllers;
using BookingSite.Application.Services;
using BookingSite.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Add services and repositories
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>(); 
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

// ✅ SECURE CORS Configuration
// Define allowed origins explicitly for production
var allowedOrigins = new[]
{
    "http://localhost:3000",
    "http://localhost:3001",
    "https://staylodgify.vercel.app",
    "https://staylodgify-frontend.vercel.app",
    "https://www.staylodgify.com",
    "https://staylodgify.com",
    "https://www.staylodgify.lat",
    "https://staylodgify.lat"
    // Add your production frontend URLs here
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow any origin for testing
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Production: Explicit origins only
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    
var jwtSettings = builder.Configuration.GetSection("Jwt");

// ✅ SECURE Authentication Configuration
// Supports both Bearer token (for mobile/API) and HttpOnly Cookie (for web)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "MultiScheme";
        options.DefaultChallengeScheme = "MultiScheme";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
            ClockSkew = TimeSpan.Zero // No tolerance for token expiration
        };
    })
    .AddJwtBearer("Cookie", options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // ✅ SECURE: Extract JWT from HttpOnly cookie
                if (context.Request.Cookies.ContainsKey("jwt"))
                {
                    context.Token = context.Request.Cookies["jwt"];
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddPolicyScheme("MultiScheme", "Bearer or Cookie", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            // Prefer cookie auth for web clients
            if (context.Request.Cookies.ContainsKey("jwt"))
                return "Cookie";
            
            // Fallback to Bearer for API/mobile clients
            return "Bearer";
        };
    });

var app = builder.Build();

// Configure HTTP pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

// ✅ CRITICAL: Middleware order matters for security!
// 1. Routing first
app.UseRouting();

// 2. CORS before Authentication
app.UseCors("AllowFrontend");

// 3. Global exception handler to ensure CORS headers on errors
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        // Don't expose internal errors in production
        var errorMessage = app.Environment.IsDevelopment() 
            ? ex.Message 
            : "An internal error occurred";
            
        await context.Response.WriteAsJsonAsync(new { 
            error = "Internal server error",
            message = errorMessage,
            timestamp = DateTime.UtcNow
        });
    }
});

// 4. Authentication
app.UseAuthentication();

// 5. ✅ SECURE: Tenant validation AFTER authentication
app.UseTenantValidation();

// 6. Authorization
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/", () => new { 
    status = "OK", 
    service = "StayLodgify API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
});

app.Run();