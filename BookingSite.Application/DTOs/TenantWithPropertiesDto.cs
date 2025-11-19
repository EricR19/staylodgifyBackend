namespace BookingSite.Application.DTOs
{
    public class TenantWithPropertiesDto : TenantDto
    {
        public List<PropertyDto> Properties { get; set; } = new();
    }
}