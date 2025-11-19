namespace BookingSite.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public int Tenant_id { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
    public string password_hash { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime created_at { get; set; }
    
    // ✅ Password Reset Fields
    public string? reset_token { get; set; }
    public DateTime? reset_token_expires { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<Logs>? Logs { get; set; }
}