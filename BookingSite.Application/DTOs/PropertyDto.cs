using System.Text.Json.Serialization;

namespace BookingSite.Application.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        
        [JsonPropertyName("tenantId")]
        public int TenantId { get; set; }
        
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        
        [JsonPropertyName("mainImage")]
        public string? MainImage { get; set; }
        
        [JsonPropertyName("otherImages")]
        public List<string>? OtherImages { get; set; }
        
        // Nuevos campos para amenidades y reglas
        [JsonPropertyName("amenities")]
        public List<string>? Amenities { get; set; }
        
        [JsonPropertyName("checkInTime")]
        public string? CheckInTime { get; set; }  // Formato "HH:mm"
        
        [JsonPropertyName("checkOutTime")]
        public string? CheckOutTime { get; set; }  // Formato "HH:mm"
        
        [JsonPropertyName("houseRules")]
        public PropertyHouseRulesDto? HouseRules { get; set; }
    }
    
    public class PropertyHouseRulesDto
    {
        [JsonPropertyName("importantInfo")]
        public List<string>? ImportantInfo { get; set; }
        
        [JsonPropertyName("customNotes")]
        public string? CustomNotes { get; set; }
    }
}

