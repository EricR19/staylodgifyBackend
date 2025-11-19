namespace BookingSite.Application.DTOs;

public class GuestCreateReservationDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int TenantId { get; set; }
}