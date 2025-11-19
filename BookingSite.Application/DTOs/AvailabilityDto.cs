namespace BookingSite.Application.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime Date { get; set; }
    public bool? IsAvailable { get; set; }
    public decimal Price { get; set; }
}