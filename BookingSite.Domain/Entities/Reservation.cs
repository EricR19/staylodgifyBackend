namespace BookingSite.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int Tenant_Id { get; set; }
    public int Room_Id { get; set; }
    public int Guest_Id { get; set; }
    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime Created_At { get; set; }

    public Tenant? Tenant { get; set; }
    public Room? Room { get; set; }
    public Guest? Guest { get; set; }
    public ICollection<Receipts>? Receipts { get; set; }
    public ICollection<Payments>? Payments { get; set; }
}