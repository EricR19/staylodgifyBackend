namespace BookingSite.Domain.Entities;

public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int TenantId { get; set; }
    
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime created_at { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
    
}