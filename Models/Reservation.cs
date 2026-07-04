namespace Hotel_Booking_System.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; }
        public int PropertyId { get; set; }
        public int? RoomTypeId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public DateTime CheckInDate { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } // Pending, Confirmed, CheckedIn, CheckedOut, Cancelled
        public string SpecialRequests { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        // Navigation Properties
        public virtual Property Property { get; set; }
        public virtual RoomType RoomType { get; set; }
    }
}
