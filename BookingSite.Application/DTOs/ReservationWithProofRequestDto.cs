using Microsoft.AspNetCore.Http;
using System;

namespace BookingSite.Application.DTOs
{
    public class ReservationWithProofRequestDto
    {
        public int RoomId { get; set; }
        public int? GuestId { get; set; }
        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public IFormFile PaymentProof { get; set; }
    }
}