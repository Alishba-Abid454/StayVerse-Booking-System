namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 6);
        Task<IEnumerable<Property>> GetPropertiesByTypeAsync(string type);
        Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string location, DateTime? checkIn, DateTime? checkOut, int? guests, string type);
        Task<IEnumerable<Property>> GetPropertiesWithAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests);
        Task<Property> GetPropertyWithDetailsAsync(int id);
        Task<Property> GetPropertyWithRoomsAsync(int id);
        Task<Property> GetPropertyWithReviewsAsync(int id);
        Task<double> GetAverageRatingAsync(int propertyId);
        Task<int> GetTotalReviewsAsync(int propertyId);
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<Property>> GetTopRatedPropertiesAsync(int count = 10);
        Task<IEnumerable<Property>> GetMostBookedPropertiesAsync(int count = 10);
        Task<Dictionary<int, double>> GetOccupancyRatesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Property>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
