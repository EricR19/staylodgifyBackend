namespace BookingSite.Application.DTOs;

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string? Error { get; set; }
    public UserInfo User { get; set; }
    public TenantInfo Tenant { get; set; }
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public int TenantId { get; set; }
}

public class TenantInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Plan { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }
}