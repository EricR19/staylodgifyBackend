using System.Text.Json.Serialization;

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
        
        [JsonPropertyName("mainImage")]
        public string MainImage { get; set; } = null!;
        
        [JsonPropertyName("otherImages")]
        public List<string>? OtherImages { get; set; }
        
        // Nuevos campos para amenidades y reglas
        [JsonPropertyName("amenities")]
        public List<string>? Amenities { get; set; }
        
        [JsonPropertyName("checkInTime")]
        public string? CheckInTime { get; set; }  // Formato "HH:mm" (default "15:00")
        
        [JsonPropertyName("checkOutTime")]
        public string? CheckOutTime { get; set; }  // Formato "HH:mm" (default "10:00")
        
        [JsonPropertyName("houseRules")]
        public PropertyHouseRulesDto? HouseRules { get; set; }
    }
}