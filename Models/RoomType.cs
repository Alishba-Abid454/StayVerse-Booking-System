namespace Hotel_Booking_System.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; } // Single Room, Double Room, Suite, etc.
        public string Description { get; set; }
        public string BedType { get; set; } // 1 single bed, 1 double bed, etc.
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal WeekendPrice { get; set; }
        public int Quantity { get; set; } // Number of rooms available
        public int AvailableQuantity { get; set; }
        public string Size { get; set; } // 25m², 40m², etc.
        public string View { get; set; } // City view, Ocean view, etc.
        public bool IsSmokingAllowed { get; set; }
        public bool IsPetFriendly { get; set; }
        public List<string> RoomAmenities { get; set; }
        public bool IsActive { get; set; }

        // Navigation Property
        public virtual Property Property { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}