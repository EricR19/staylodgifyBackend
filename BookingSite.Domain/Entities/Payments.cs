namespace BookingSite.Domain.Entities;

public class Payments
{
    public int Id { get; set; }
    public int Tenant_Id { get; set; }
    public int Reservation_Id { get; set; }
    public string? Payment_Type { get; set; }
    public string Provider { get; set; } = null!;
    public string Provider_Payment_Id { get; set; } = null!;
    public string? Card_Last4 { get; set; }
    public string? Card_Brand { get; set; }
    public string? Receipt_Url { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "pending";
    public DateTime? Paid_At { get; set; }
    public DateTime Created_At { get; set; }

    public Tenant? Tenant { get; set; }
    public Reservation? Reservation { get; set; }
}