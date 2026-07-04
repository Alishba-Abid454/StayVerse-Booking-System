namespace Hotel_Booking_System.Models.ViewModels
{
    public class ReportViewModel
    {
        public string ReportType { get; set; } // Revenue, Booking, Occupancy, etc.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; } // Daily, Weekly, Monthly, Yearly

        // Summary
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalGuests { get; set; }
        public double AverageDailyRate { get; set; }
        public double OccupancyRate { get; set; }
        public decimal RevenuePerAvailableRoom { get; set; }

        // Breakdown
        public List<RevenueReportItem> RevenueBreakdown { get; set; }
        public List<BookingReportItem> BookingBreakdown { get; set; }
        public List<PropertyReportItem> PropertyPerformance { get; set; }
        public List<ChannelReportItem> ChannelPerformance { get; set; }
    }

    public class RevenueReportItem
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
        public int Bookings { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class BookingReportItem
    {
        public string Date { get; set; }
        public int Bookings { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
        public int Pending { get; set; }
        public decimal Revenue { get; set; }
    }

    public class PropertyReportItem
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double OccupancyRate { get; set; }
        public double Rating { get; set; }
    }

    public class ChannelReportItem
    {
        public string Channel { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double ConversionRate { get; set; }
        public decimal Commission { get; set; }
        public decimal NetRevenue { get; set; }
    }
}
