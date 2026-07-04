using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId)
        {
            return await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 10)
        {
            return await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByRatingAsync(double minRating, double maxRating)
        {
            return await _context.Reviews
                .Where(r => r.Rating >= minRating && r.Rating <= maxRating)
                .OrderByDescending(r => r.Rating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetVerifiedReviewsAsync()
        {
            return await _context.Reviews
                .Where(r => r.IsVerified)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetRecommendedReviewsAsync()
        {
            return await _context.Reviews
                .Where(r => r.IsRecommended)
                .OrderByDescending(r => r.Rating)
                .ToListAsync();
        }

        public async Task<Review> GetReviewWithDetailsAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<double> GetAverageRatingByPropertyAsync(int propertyId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<Dictionary<string, double>> GetCategoryRatingsAsync(int propertyId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();

            var result = new Dictionary<string, double>();

            if (reviews.Any())
            {
                result["Staff"] = reviews.Average(r => r.StaffRating);
                result["Facilities"] = reviews.Average(r => r.FacilitiesRating);
                result["Cleanliness"] = reviews.Average(r => r.CleanlinessRating);
                result["Comfort"] = reviews.Average(r => r.ComfortRating);
                result["ValueForMoney"] = reviews.Average(r => r.ValueForMoneyRating);
                result["Location"] = reviews.Average(r => r.LocationRating);
            }

            return result;
        }

        public async Task<int> GetTotalReviewsByPropertyAsync(int propertyId)
        {
            return await _context.Reviews
                .CountAsync(r => r.PropertyId == propertyId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reviews
                .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserReviewedPropertyAsync(string userId, int propertyId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.PropertyId == propertyId);
        }

        public async Task<IEnumerable<Review>> GetHelpfulReviewsAsync(int minHelpfulCount = 5)
        {
            return await _context.Reviews
                .Where(r => r.HelpfulCount >= minHelpfulCount)
                .OrderByDescending(r => r.HelpfulCount)
                .ToListAsync();
        }

        public async Task<Review> GetLatestReviewAsync(int propertyId)
        {
            return await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
