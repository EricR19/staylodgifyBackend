namespace BookingSite.Application.DTOs
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}