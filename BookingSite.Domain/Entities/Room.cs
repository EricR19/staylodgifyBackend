namespace BookingSite.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public int property_id { get; set; }
    public string room_number { get; set; } = null!;
    public string description { get; set; } = null!;
    public int capacity { get; set; }
    public decimal price_per_night { get; set; }
    public string? images { get; set; }
    public DateTime created_at { get; set; }

    public Property? Property { get; set; }
    public ICollection<Availability>? Availability { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
}