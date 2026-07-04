using Hotel_Booking_System.Controllers;

namespace Hotel_Booking_System.Models.ViewModels
{
    public class SearchViewModel
    {
        // Search Criteria
        public string Location { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Guests { get; set; }
        public int? Rooms { get; set; }
        public string PropertyType { get; set; } // Hotel, Apartment, Restaurant, All
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRating { get; set; }
        public List<string> SelectedAmenities { get; set; }
        public string SortBy { get; set; } // PriceLow, PriceHigh, Rating, Popularity, Newest

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Results
        public List<PropertyViewModel> Results { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalResults / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Filter Options
        public List<string> AvailablePropertyTypes { get; set; }
        public List<string> AvailableAmenities { get; set; }
        public decimal MinAvailablePrice { get; set; }
        public decimal MaxAvailablePrice { get; set; }

        // Search Metadata
        public string SearchTerm { get; set; }
        public bool IsSearchPerformed { get; set; }
        public TimeSpan? SearchDuration { get; set; }
    }
}
