using System.Collections.Generic;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class RoomTypeViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BedType { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal WeekendPrice { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Size { get; set; }
        public string View { get; set; }
        public bool IsSmokingAllowed { get; set; }
        public bool IsPetFriendly { get; set; }
        public List<string> RoomAmenities { get; set; }
        public List<string> Images { get; set; }
        public List<System.DateTime> AvailableDates { get; set; }
        public bool IsAvailable { get; set; }
    }
}