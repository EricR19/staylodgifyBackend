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
    
    // Nuevos campos para amenidades y reglas
    public string? Amenities { get; set; }  // JSON array de amenidades
    public TimeSpan? Check_In_Time { get; set; }  // Hora de entrada (default 15:00)
    public TimeSpan? Check_Out_Time { get; set; }  // Hora de salida (default 10:00)
    public string? House_Rules { get; set; }  // JSON con reglas de la propiedad
    
    public DateTime Created_At { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<Room>? Rooms { get; set; }
}