using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.Interfaces;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPropertyRepository _propertyRepository;

        public ReviewService(IReviewRepository reviewRepository, IPropertyRepository propertyRepository)
        {
            _reviewRepository = reviewRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<ReviewViewModel>> GetReviewsByPropertyAsync(int propertyId)
        {
            var reviews = await _reviewRepository.GetReviewsByPropertyAsync(propertyId);
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReviewViewModel>> GetReviewsByUserAsync(string userId)
        {
            var reviews = await _reviewRepository.GetReviewsByUserAsync(userId);
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReviewViewModel>> GetRecentReviewsAsync(int count = 10)
        {
            var reviews = await _reviewRepository.GetRecentReviewsAsync(count);
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<ReviewViewModel> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetReviewWithDetailsAsync(id);
            return review != null ? MapToViewModel(review) : null;
        }

        public async Task<ReviewViewModel> CreateReviewAsync(ReviewViewModel model)
        {
            // Check if user already reviewed this property
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var hasReviewed = await HasUserReviewedPropertyAsync(model.UserId, model.PropertyId);
                if (hasReviewed)
                    throw new Exception("You have already reviewed this property");
            }

            var review = new Review
            {
                PropertyId = (int)model.PropertyId,
                UserId = model.UserId,
                UserName = model.UserName,
                UserEmail = model.UserEmail,
                Country = model.Country,
                Rating = model.Rating,
                StaffRating = model.StaffRating > 0 ? model.StaffRating : model.Rating,
                FacilitiesRating = model.FacilitiesRating > 0 ? model.FacilitiesRating : model.Rating,
                CleanlinessRating = model.CleanlinessRating > 0 ? model.CleanlinessRating : model.Rating,
                ComfortRating = model.ComfortRating > 0 ? model.ComfortRating : model.Rating,
                ValueForMoneyRating = model.ValueForMoneyRating > 0 ? model.ValueForMoneyRating : model.Rating,
                LocationRating = model.LocationRating > 0 ? model.LocationRating : model.Rating,
                Title = model.Title,
                Comment = model.Comment,
                Pros = model.Pros,
                Cons = model.Cons,
                IsVerified = true,
                IsRecommended = model.Rating >= 7,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            // Update property rating
            await UpdatePropertyRatingAsync(model.PropertyId);

            return MapToViewModel(review);
        }

        private async Task UpdatePropertyRatingAsync(object propertyId)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> HasUserReviewedPropertyAsync(string userId, object propertyId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateReviewAsync(ReviewViewModel model)
        {
            var review = await _reviewRepository.GetByIdAsync(model.Id);
            if (review == null)
                throw new Exception("Review not found");

            review.Rating = model.Rating;
            review.StaffRating = model.StaffRating;
            review.FacilitiesRating = model.FacilitiesRating;
            review.CleanlinessRating = model.CleanlinessRating;
            review.ComfortRating = model.ComfortRating;
            review.ValueForMoneyRating = model.ValueForMoneyRating;
            review.LocationRating = model.LocationRating;
            review.Title = model.Title;
            review.Comment = model.Comment;
            review.Pros = model.Pros;
            review.Cons = model.Cons;
            review.IsRecommended = model.Rating >= 7;
            review.UpdatedAt = DateTime.UtcNow;

            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();

            // Update property rating
            await UpdatePropertyRatingAsync(review.PropertyId);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review != null)
            {
                var propertyId = review.PropertyId;
                _reviewRepository.Delete(review);
                await _reviewRepository.SaveChangesAsync();

                // Update property rating
                await UpdatePropertyRatingAsync(propertyId);
            }
        }

        public async Task<double> GetAverageRatingByPropertyAsync(int propertyId)
        {
            return await _reviewRepository.GetAverageRatingByPropertyAsync(propertyId);
        }

        public async Task<Dictionary<string, double>> GetCategoryRatingsAsync(int propertyId)
        {
            return await _reviewRepository.GetCategoryRatingsAsync(propertyId);
        }

        public async Task<int> GetTotalReviewsByPropertyAsync(int propertyId)
        {
            return await _reviewRepository.GetTotalReviewsByPropertyAsync(propertyId);
        }

        public async Task<IEnumerable<ReviewViewModel>> GetReviewsByRatingAsync(double minRating, double maxRating)
        {
            var reviews = await _reviewRepository.GetReviewsByRatingAsync(minRating, maxRating);
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReviewViewModel>> GetVerifiedReviewsAsync()
        {
            var reviews = await _reviewRepository.GetVerifiedReviewsAsync();
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReviewViewModel>> GetRecommendedReviewsAsync()
        {
            var reviews = await _reviewRepository.GetRecommendedReviewsAsync();
            return reviews.Select(r => MapToViewModel(r));
        }

        public async Task<bool> HasUserReviewedPropertyAsync(string userId, int propertyId)
        {
            return await _reviewRepository.HasUserReviewedPropertyAsync(userId, propertyId);
        }

        public async Task<ReviewViewModel> GetLatestReviewAsync(int propertyId)
        {
            var review = await _reviewRepository.GetLatestReviewAsync(propertyId);
            return review != null ? MapToViewModel(review) : null;
        }

        #region Private Methods

        private async Task UpdatePropertyRatingAsync(int propertyId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null) return;

            var averageRating = await GetAverageRatingByPropertyAsync(propertyId);
            var totalReviews = await GetTotalReviewsByPropertyAsync(propertyId);

            property.Rating = Math.Round(averageRating, 1);
            property.TotalReviews = totalReviews;
            property.UpdatedAt = DateTime.UtcNow;

            _propertyRepository.Update(property);
            await _propertyRepository.SaveChangesAsync();
        }

        #endregion

        #region Mapping Methods

        private ReviewViewModel MapToViewModel(Review review)
        {
            return new ReviewViewModel
            {
                Id = review.Id,
                PropertyId = review.PropertyId,
                UserId = review.UserId,
                UserName = review.UserName,
                UserEmail = review.UserEmail,
                Country = review.Country,
                Rating = review.Rating,
                StaffRating = review.StaffRating,
                FacilitiesRating = review.FacilitiesRating,
                CleanlinessRating = review.CleanlinessRating,
                ComfortRating = review.ComfortRating,
                ValueForMoneyRating = review.ValueForMoneyRating,
                LocationRating = review.LocationRating,
                Title = review.Title,
                Comment = review.Comment,
                Pros = review.Pros,
                Cons = review.Cons,
                Date = review.CreatedAt.ToString("MMM dd, yyyy"),
                ReviewDate = review.CreatedAt,
                IsVerified = review.IsVerified,
                IsRecommended = review.IsRecommended,
                HelpfulCount = review.HelpfulCount
            };
        }

        #endregion
    }
}