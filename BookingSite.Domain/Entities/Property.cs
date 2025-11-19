namespace BookingSite.Domain.Entities;

public class Property
{
    public int Id { get; set; }
    public int Tenant_Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Main_Image { get; set; } = null!;
    public string? Other_Images { get; set; }
    public DateTime Created_At { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<Room>? Rooms { get; set; }
}