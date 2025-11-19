namespace BookingSite.Application.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public List<string>? Images { get; set; }
    }
}