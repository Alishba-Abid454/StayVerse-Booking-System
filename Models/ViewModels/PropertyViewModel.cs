using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class PropertyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Property Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Property Type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsActive { get; set; }

        public string Price { get; set; }

        [Required(ErrorMessage = "Base Price is required")]
        public decimal BasePrice { get; set; }

        public string Currency { get; set; }

        public string ImageUrl { get; set; }

        public string? ThumbnailUrl { get; set; }

        public List<string> GalleryImages { get; set; } = new List<string>();

        [Required(ErrorMessage = "Short Description is required")]
        public string Description { get; set; }

        public string FullDescription { get; set; }

        public string AmenitiesInput { get; set; }

        public List<string> Amenities { get; set; } = new List<string>();

        public List<string> Highlights { get; set; } = new List<string>();
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public List<RoomTypeViewModel> RoomTypes { get; set; } = new List<RoomTypeViewModel>();

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "Minimum Nights is required")]
        public int MinNights { get; set; }

        [Required(ErrorMessage = "Max Guests is required")]
        public int MaxGuests { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OccupancyRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal EstimatedRevenue { get; set; }
        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

        public string GalleryImagesInput { get; set; }
        public string? RemovedGalleryImages { get; internal set; }
    }
}