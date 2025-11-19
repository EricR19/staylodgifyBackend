namespace BookingSite.Application.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public GuestDto Guest { get; set; }
}