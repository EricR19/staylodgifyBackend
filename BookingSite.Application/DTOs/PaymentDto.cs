namespace BookingSite.Application.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int Reservation_Id { get; set; }
        public string? Payment_Type { get; set; }
        public string Provider { get; set; }
        public string Provider_Payment_Id { get; set; }
        public string? Card_Last4 { get; set; }
        public string? Card_Brand { get; set; }
        public string? Receipt_Url { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime? Paid_At { get; set; }
        public DateTime Created_At { get; set; }
    }
}