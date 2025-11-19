namespace BookingSite.Domain.Entities;

public class Receipts
{
    public int Id { get; set; }
    public int Reservation_Id { get; set; }
    public string File_Url { get; set; } = null!;
    public DateTime Uploaded_At { get; set; }

    public Reservation? Reservation { get; set; }
}