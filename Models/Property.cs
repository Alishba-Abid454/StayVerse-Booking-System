using System;
using System.Collections.Generic;

namespace Hotel_Booking_System.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public string Price { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }

        public string ImageUrl { get; set; }

        public string? ThumbnailUrl { get; set; }

        public List<string> GalleryImages { get; set; } = new List<string>();

        public int MinNights { get; set; }
        public int MaxGuests { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<PropertyAmenity> Amenities { get; set; }
        public virtual ICollection<RoomType> RoomTypes { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}