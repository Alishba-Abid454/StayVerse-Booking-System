using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.Interfaces;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserWithReservationsAsync(string userId)
        {
            return await _userRepository.GetUserWithReservationsAsync(userId);
        }

        public async Task<User> GetUserWithReviewsAsync(string userId)
        {
            return await _userRepository.GetUserWithReviewsAsync(userId);
        }

        public async Task<ProfileViewModel> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserWithReservationsAsync(userId);
            if (user == null)
                return null;

            var profile = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                City = user.City,
                Address = user.Address,
                PostalCode = user.PostalCode,
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                LoyaltyPoints = user.LoyaltyPoints,
                TotalBookings = user.Reservations?.Count(r => r.Status != "Cancelled") ?? 0,
                MemberSince = user.CreatedAt.ToString("MMMM yyyy"),
                IsEmailVerified = user.EmailConfirmed,
                IsPhoneVerified = false
            };

            // Add upcoming and past bookings
            if (user.Reservations != null)
            {
                var now = DateTime.Now;
                profile.UpcomingBookings = user.Reservations
                    .Where(r => r.CheckInDate >= now && r.Status != "Cancelled")
                    .Select(r => MapToReservationViewModel(r))
                    .ToList();

                profile.PastBookings = user.Reservations
                    .Where(r => r.CheckOutDate < now || r.Status == "Cancelled")
                    .Select(r => MapToReservationViewModel(r))
                    .ToList();
            }

            return profile;
        }

        public async Task<ProfileViewModel> UpdateUserProfileAsync(ProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                throw new Exception("User not found");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Country = model.Country;
            user.City = model.City;
            user.Address = model.Address;
            user.PostalCode = model.PostalCode;
            user.ProfileImageUrl = model.ProfileImageUrl;
            user.DateOfBirth = model.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Failed to update user profile");

            return await GetUserProfileAsync(model.Id);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _userRepository.GetActiveUsersAsync();
        }

        public async Task<IEnumerable<User>> GetInactiveUsersAsync()
        {
            return await _userRepository.GetInactiveUsersAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.GetTotalUsersAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userRepository.GetActiveUsersCountAsync();
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _userRepository.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsPhoneExistsAsync(string phoneNumber)
        {
            return await _userRepository.IsPhoneExistsAsync(phoneNumber);
        }

        public async Task UpdateUserStatusAsync(string userId, bool isActive)
        {
            await _userRepository.UpdateUserStatusAsync(userId, isActive);
        }

        public async Task<int> GetUserBookingsCountAsync(string userId)
        {
            return await _userRepository.GetUserBookingsCountAsync(userId);
        }

        public async Task<decimal> GetUserTotalSpentAsync(string userId)
        {
            return await _userRepository.GetUserTotalSpentAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersWithMostBookingsAsync(int count = 10)
        {
            return await _userRepository.GetUsersWithMostBookingsAsync(count);
        }

        public async Task<Dictionary<string, int>> GetUserRegistrationStatsAsync(DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUserRegistrationStatsAsync(startDate, endDate);
        }

        #region Private Methods

        private ReservationViewModel MapToReservationViewModel(Reservation reservation)
        {
            return new ReservationViewModel
            {
                Id = reservation.Id,
                ReservationNumber = reservation.ReservationNumber,
                PropertyId = reservation.PropertyId,
                PropertyName = reservation.Property?.Name,
                PropertyImage = reservation.Property?.ImageUrl,
                PropertyType = reservation.Property?.Type,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                NumberOfGuests = reservation.NumberOfGuests,
                NumberOfRooms = reservation.NumberOfRooms,
                TotalPrice = reservation.TotalPrice,
                PricePerNight = reservation.PricePerNight,
                Currency = reservation.Currency,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt
            };
        }

        #endregion
    }
}