using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class BookingViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string PropertyType { get; set; }
        public string PropertyLocation { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public int? SelectedRoomTypeId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; }
        public string SpecialRequests { get; set; }
        public List<SelectListItem> RoomTypes { get; set; }
        public List<RoomTypeViewModel> AvailableRooms { get; set; }

        // User Information
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        // Payment Information
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }
        public bool SavePaymentInfo { get; set; }

        // Terms
        public bool AcceptTerms { get; set; }

        // Price Breakdown
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}