namespace Hotel_Booking_System.Models.ViewModels
{
    public class PropertyAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double OccupancyRate { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalRooms { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string StatusColor
        {
            get
            {
                return Status switch
                {
                    "Active" => "#059669",
                    "Inactive" => "#dc2626",
                    "Pending" => "#f59e0b",
                    _ => "#6b7280"
                };
            }
        }
        public List<RecentReservationViewModel> RecentBookings { get; set; }
        public List<ReviewViewModel> RecentReviews { get; set; }
    }
}
