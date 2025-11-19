namespace BookingSite.Domain.Entities;

public class Logs
{
    public int Id { get; set; }
    public int? User_Id { get; set; }
    public int Tenant_Id { get; set; }
    public string Action { get; set; } = null!;
    public DateTime Created_At { get; set; }

    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}