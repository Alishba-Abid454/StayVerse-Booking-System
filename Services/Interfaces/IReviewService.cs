using Hotel_Booking_System.Models.ViewModels;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewViewModel>> GetReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<ReviewViewModel>> GetReviewsByUserAsync(string userId);
        Task<IEnumerable<ReviewViewModel>> GetRecentReviewsAsync(int count = 10);
        Task<ReviewViewModel> GetReviewByIdAsync(int id);
        Task<ReviewViewModel> CreateReviewAsync(ReviewViewModel model);
        Task UpdateReviewAsync(ReviewViewModel model);
        Task DeleteReviewAsync(int id);
        Task<double> GetAverageRatingByPropertyAsync(int propertyId);
        Task<Dictionary<string, double>> GetCategoryRatingsAsync(int propertyId);
        Task<int> GetTotalReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<ReviewViewModel>> GetReviewsByRatingAsync(double minRating, double maxRating);
        Task<IEnumerable<ReviewViewModel>> GetVerifiedReviewsAsync();
        Task<IEnumerable<ReviewViewModel>> GetRecommendedReviewsAsync();
        Task<bool> HasUserReviewedPropertyAsync(string userId, int propertyId);
        Task<ReviewViewModel> GetLatestReviewAsync(int propertyId);
    }
}
