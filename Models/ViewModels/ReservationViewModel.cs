using System;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string ReservationNumber { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string PropertyType { get; set; }
        public string PropertyLocation { get; set; }
        public int? RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomTypeImage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string SpecialRequests { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; }
        public bool CanCancel => Status == "Confirmed" && (CheckInDate - DateTime.Now).TotalHours > 24;
        public bool CanModify => Status == "Confirmed" && (CheckInDate - DateTime.Now).TotalHours > 48;
        public string StatusColor
        {
            get
            {
                return Status switch
                {
                    "Pending" => "#f59e0b",
                    "Confirmed" => "#2563eb",
                    "CheckedIn" => "#059669",
                    "CheckedOut" => "#6b7280",
                    "Cancelled" => "#dc2626",
                    _ => "#6b7280"
                };
            }
        }
    }
}