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
    }
}