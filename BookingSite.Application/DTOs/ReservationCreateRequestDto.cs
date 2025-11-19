namespace BookingSite.Application.DTOs
{
    public class ReservationCreateRequestDto
    {
        public int RoomId { get; set; }
        public int? GuestId { get; set; }
        public GuestCreateReservationDto Guest { get; set; } // Para entrada
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}