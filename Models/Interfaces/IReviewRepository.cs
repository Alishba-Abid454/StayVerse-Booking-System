namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId);
        Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 10);
        Task<IEnumerable<Review>> GetReviewsByRatingAsync(double minRating, double maxRating);
        Task<IEnumerable<Review>> GetVerifiedReviewsAsync();
        Task<IEnumerable<Review>> GetRecommendedReviewsAsync();
        Task<Review> GetReviewWithDetailsAsync(int id);
        Task<double> GetAverageRatingByPropertyAsync(int propertyId);
        Task<Dictionary<string, double>> GetCategoryRatingsAsync(int propertyId);
        Task<int> GetTotalReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<Review>> GetReviewsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> HasUserReviewedPropertyAsync(string userId, int propertyId);
        Task<IEnumerable<Review>> GetHelpfulReviewsAsync(int minHelpfulCount = 5);
        Task<Review> GetLatestReviewAsync(int propertyId);
    }
}
