namespace BookingSite.Application.DTOs
{
    public class TenantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Plan { get; set; }
        public string Status { get; set; }
        public DateTime? SubscriptionExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}