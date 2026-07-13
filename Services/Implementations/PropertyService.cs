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
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<PropertyViewModel>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PropertyViewModel>> GetFeaturedPropertiesAsync(int count = 6)
        {
            var properties = await _propertyRepository.GetFeaturedPropertiesAsync(count);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<PropertyViewModel> GetPropertyByIdAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            return property != null ? MapToViewModel(property) : null;
        }

        public async Task<PropertyViewModel> GetPropertyWithDetailsAsync(int id)
        {
            var property = await _propertyRepository.GetPropertyWithDetailsAsync(id);
            if (property == null) return null;

            var viewModel = MapToViewModel(property);
            viewModel.RoomTypes = property.RoomTypes?.Select(r => MapToRoomTypeViewModel(r)).ToList();
            viewModel.Reviews = property.Reviews?.Select(r => MapToReviewViewModel(r)).ToList();
            viewModel.Amenities = property.Amenities?.Select(a => a.Name).ToList();

            return viewModel;
        }

        public async Task<IEnumerable<PropertyViewModel>> SearchPropertiesAsync(string location, string type, int? guests)
        {
            var properties = await _propertyRepository.SearchPropertiesAsync(location, null, null, guests, type);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PropertyViewModel>> GetPropertiesByTypeAsync(string type)
        {
            var properties = await _propertyRepository.GetPropertiesByTypeAsync(type);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PropertyViewModel>> GetPropertiesByCityAsync(string city)
        {
            var properties = await _propertyRepository.GetPropertiesByCityAsync(city);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PropertyViewModel>> GetTopRatedPropertiesAsync(int count = 10)
        {
            var properties = await _propertyRepository.GetTopRatedPropertiesAsync(count);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PropertyViewModel>> GetMostBookedPropertiesAsync(int count = 10)
        {
            var properties = await _propertyRepository.GetMostBookedPropertiesAsync(count);
            return properties.Select(p => MapToViewModel(p));
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            return await _propertyRepository.IsPropertyAvailableAsync(propertyId, checkIn, checkOut);
        }
        public async Task<PropertyViewModel> CreatePropertyAsync(PropertyViewModel model)
        {
            var property = new Property
            {
                Name = model.Name,
                Type = model.Type,
                Description = model.Description,
                FullDescription = model.FullDescription,
                Location = model.Location,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                PostalCode = model.PostalCode ?? "00000",
                Price = model.Price,
                BasePrice = model.BasePrice,
                Currency = model.Currency ?? "USD",
                ImageUrl = model.ImageUrl,

                // ✅ ADD THIS LINE - ThumbnailUrl
                ThumbnailUrl = model.ThumbnailUrl ?? model.ImageUrl ?? "/images/default-property.jpg",

                // ✅ ADD THIS LINE - GalleryImages
                GalleryImages = model.GalleryImages ?? new List<string>(),

                Rating = 0,
                TotalReviews = 0,
                MinNights = model.MinNights > 0 ? model.MinNights : 1,
                MaxGuests = model.MaxGuests > 0 ? model.MaxGuests : 4,
                IsActive = true,
                IsFeatured = model.IsFeatured,
                CreatedAt = DateTime.UtcNow,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            await _propertyRepository.AddAsync(property);
            await _propertyRepository.SaveChangesAsync();
/*            var room = new RoomType
            {
                PropertyId = property.Id,
                Name = "Standard Room",
                Description = "Default Room",
                BedType = "Double",
                MaxGuests = property.MaxGuests,
                Bedrooms = 1,
                Bathrooms = 1,
                PricePerNight = property.BasePrice,
                WeekendPrice = property.BasePrice,
                Quantity = 10,
                AvailableQuantity = 10,
                Size = "250 sq ft"
            };

            await _propertyRepository.AddRoomTypeAsync(room);*/

            return MapToViewModel(property);
        }

        public async Task UpdatePropertyAsync(PropertyViewModel model)
        {
            var property = await _propertyRepository.GetByIdAsync(model.Id);
            if (property == null)
                throw new Exception("Property not found");

            property.Name = model.Name;
            property.Type = model.Type;
            property.Description = model.Description;
            property.FullDescription = model.FullDescription;
            property.Location = model.Location;
            property.Address = model.Address;
            property.City = model.City;
            property.Country = model.Country;
            property.PostalCode = model.PostalCode ?? "00000";
            property.Price = model.Price;
            property.BasePrice = model.BasePrice;
            property.Currency = model.Currency ?? "USD";
            property.ImageUrl = model.ImageUrl;
            property.ThumbnailUrl = model.ThumbnailUrl ?? model.ImageUrl ?? "/images/default-property.jpg";

            // ✅ Update Gallery Images
            property.GalleryImages = model.GalleryImages ?? new List<string>();

            property.MinNights = model.MinNights;
            property.MaxGuests = model.MaxGuests;
            property.IsFeatured = model.IsFeatured;
            property.UpdatedAt = DateTime.UtcNow;

            _propertyRepository.Update(property);
            await _propertyRepository.SaveChangesAsync();
        }

        public async Task DeletePropertyAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property != null)
            {
                _propertyRepository.Delete(property);
                await _propertyRepository.SaveChangesAsync();
            }
        }

        public async Task<double> GetAverageRatingAsync(int propertyId)
        {
            return await _propertyRepository.GetAverageRatingAsync(propertyId);
        }

        public async Task<Dictionary<int, double>> GetOccupancyRatesAsync(DateTime startDate, DateTime endDate)
        {
            return await _propertyRepository.GetOccupancyRatesAsync(startDate, endDate);
        }

        public async Task<IEnumerable<PropertyViewModel>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var properties = await _propertyRepository.GetPropertiesByPriceRangeAsync(minPrice, maxPrice);
            return properties.Select(p => MapToViewModel(p));
        }

        #region Mapping Methods

        private PropertyViewModel MapToViewModel(Property property)
        {
            return new PropertyViewModel
            {
                Id = property.Id,
                Name = property.Name,
                Type = property.Type,
                Location = property.Location,
                Address = property.Address,
                City = property.City,
                Country = property.Country,
                Rating = property.Rating,
                TotalReviews = property.TotalReviews,
                Price = property.Price,
                BasePrice = property.BasePrice,
                Currency = property.Currency,
                ImageUrl = property.ImageUrl,
                Description = property.Description,
                FullDescription = property.FullDescription,
                IsFeatured = property.IsFeatured,
                MinNights = property.MinNights,
                MaxGuests = property.MaxGuests,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                GalleryImages = property.GalleryImages ?? new List<string>(),
                OccupancyRate = 0,
                EstimatedRevenue = 0
            };
        }

        private RoomTypeViewModel MapToRoomTypeViewModel(RoomType roomType)
        {
            return new RoomTypeViewModel
            {
                Id = roomType.Id,
                PropertyId = roomType.PropertyId,
                Name = roomType.Name,
                Description = roomType.Description,
                BedType = roomType.BedType,
                MaxGuests = roomType.MaxGuests,
                Bedrooms = roomType.Bedrooms,
                Bathrooms = roomType.Bathrooms,
                PricePerNight = roomType.PricePerNight,
                WeekendPrice = roomType.WeekendPrice,
                Quantity = roomType.Quantity,
                AvailableQuantity = roomType.AvailableQuantity,
                Size = roomType.Size,
                View = roomType.View,
                IsSmokingAllowed = roomType.IsSmokingAllowed,
                IsPetFriendly = roomType.IsPetFriendly,
                RoomAmenities = roomType.RoomAmenities ?? new List<string>(),
                IsAvailable = roomType.AvailableQuantity > 0
            };
        }

        private ReviewViewModel MapToReviewViewModel(Review review)
        {
            return new ReviewViewModel
            {
                Id = review.Id,
                UserName = review.UserName,
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
                IsVerified = review.IsVerified,
                IsRecommended = review.IsRecommended
            };
        }

        #endregion
    }
}