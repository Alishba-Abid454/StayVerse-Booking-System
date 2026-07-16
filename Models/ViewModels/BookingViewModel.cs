using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class BookingViewModel
    {
        // ===== Property Information (Display Only) =====
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string PropertyType { get; set; }
        public string PropertyLocation { get; set; }
        public decimal PricePerNight { get; set; }

        // ===== Date & Guest Information (REQUIRED) =====
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }

        // ===== Room Selection (REQUIRED) =====
        public int? SelectedRoomTypeId { get; set; }

        // ===== User Information (REQUIRED - Only these) =====
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // ===== User Information (OPTIONAL) =====
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string SpecialRequests { get; set; }

        // ===== Pricing Information (Display Only) =====
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal GrandTotal { get; set; }

        // ===== Display Lists (DO NOT SUBMIT) =====
        [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
        public List<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>();

        [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
        public List<RoomTypeViewModel> AvailableRooms { get; set; } = new List<RoomTypeViewModel>();
    }
}
