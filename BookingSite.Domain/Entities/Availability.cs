namespace BookingSite.Domain.Entities;

public class Availability
{
    public int Id { get; set; }
    public int room_id { get; set; }
    public DateTime Date { get; set; }
    public bool? is_available { get; set; }
    public decimal Price { get; set; }

    public Room? Room { get; set; }
}