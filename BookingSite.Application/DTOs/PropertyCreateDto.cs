namespace BookingSite.Application.DTOs
{
    public class PropertyCreateDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string MainImage { get; set; } = null!;
        public List<string>? OtherImages { get; set; }
        
        // Nuevos campos para amenidades y reglas
        public List<string>? Amenities { get; set; }
        public string? CheckInTime { get; set; }  // Formato "HH:mm" (default "15:00")
        public string? CheckOutTime { get; set; }  // Formato "HH:mm" (default "10:00")
        public PropertyHouseRulesDto? HouseRules { get; set; }
    }
}