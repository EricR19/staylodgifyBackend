namespace BookingSite.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Contact_email { get; set; } = null!;
    public string Plan { get; set; } = null!;
    public string Status { get; set; } = "active";
    public DateTime? Subscription_expires_at { get; set; }
    public DateTime Created_at { get; set; }

    public ICollection<Property>? properties { get; set; }
    public ICollection<User>? users { get; set; }
 
    public ICollection<Reservation>? reservations { get; set; }
    public ICollection<Payments>? payments { get; set; }
    public ICollection<Logs>? logs { get; set; }
}
