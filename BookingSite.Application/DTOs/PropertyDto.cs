namespace BookingSite.Application.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? MainImage { get; set; }
        public List<string>? OtherImages { get; set; }
        
        // Nuevos campos para amenidades y reglas
        public List<string>? Amenities { get; set; }
        public string? CheckInTime { get; set; }  // Formato "HH:mm"
        public string? CheckOutTime { get; set; }  // Formato "HH:mm"
        public PropertyHouseRulesDto? HouseRules { get; set; }
    }
    
    public class PropertyHouseRulesDto
    {
        public List<string>? ImportantInfo { get; set; }
        public string? CustomNotes { get; set; }
    }
}