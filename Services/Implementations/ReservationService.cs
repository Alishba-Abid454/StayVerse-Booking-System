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
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IPropertyRepository propertyRepository,
            IRoomTypeRepository roomTypeRepository)
        {
            _reservationRepository = reservationRepository;
            _propertyRepository = propertyRepository;
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<ReservationViewModel> CreateReservationAsync(BookingViewModel model, string userId)
        {
            // Validate dates
            if (model.CheckInDate >= model.CheckOutDate)
                throw new Exception("Check-in date must be before check-out date");

            if (model.CheckInDate < DateTime.Now.Date)
                throw new Exception("Cannot book for past dates");

            // Check availability
            var isAvailable = await CheckAvailabilityAsync(
                model.PropertyId,
                model.SelectedRoomTypeId,
                model.CheckInDate,
                model.CheckOutDate);

            if (!isAvailable)
                throw new Exception("Property or room type is not available for the selected dates");

            var property = await _propertyRepository.GetByIdAsync(model.PropertyId);
            if (property == null)
                throw new Exception("Property not found");

            // Calculate total price
            var nights = (model.CheckOutDate - model.CheckInDate).Days;
            var totalPrice = model.PricePerNight * nights;

            // Apply discounts or taxes if needed
            // totalPrice = ApplyDiscounts(totalPrice, userId);

            var reservation = new Reservation
            {
                ReservationNumber = await GenerateReservationNumberAsync(),
                PropertyId = model.PropertyId,
                RoomTypeId = model.SelectedRoomTypeId,
                UserId = userId,
                UserName = model.FullName,
                UserEmail = model.Email,
                UserPhone = model.PhoneNumber,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                NumberOfRooms = model.NumberOfRooms,
                TotalPrice = totalPrice,
                PricePerNight = model.PricePerNight,
                Currency = "USD",
                Status = "Pending",
                SpecialRequests = model.SpecialRequests,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            return MapToViewModel(reservation);
        }

        public async Task<ReservationViewModel> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationWithDetailsAsync(id);
            return reservation != null ? MapToViewModel(reservation) : null;
        }

        public async Task<ReservationViewModel> GetReservationByNumberAsync(string reservationNumber)
        {
            var reservation = await _reservationRepository.GetReservationByNumberAsync(reservationNumber);
            return reservation != null ? MapToViewModel(reservation) : null;
        }

        public async Task<IEnumerable<ReservationViewModel>> GetUserReservationsAsync(string userId)
        {
            var reservations = await _reservationRepository.GetReservationsByUserAsync(userId);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetUserReservationsByEmailAsync(string email)
        {
            var reservations = await _reservationRepository.GetReservationsByUserEmailAsync(email);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetPropertyReservationsAsync(int propertyId)
        {
            var reservations = await _reservationRepository.GetReservationsByPropertyAsync(propertyId);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetActiveReservationsAsync()
        {
            var reservations = await _reservationRepository.GetActiveReservationsAsync();
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetUpcomingReservationsAsync(int days = 7)
        {
            var reservations = await _reservationRepository.GetUpcomingReservationsAsync(days);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var reservations = await _reservationRepository.GetReservationsByDateRangeAsync(startDate, endDate);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetReservationsByStatusAsync(string status)
        {
            var reservations = await _reservationRepository.GetReservationsByStatusAsync(status);
            return reservations.Select(r => MapToViewModel(r));
        }

        public async Task<bool> CancelReservationAsync(int reservationId, string reason)
        {
            var reservation = await _reservationRepository.GetReservationWithDetailsAsync(reservationId);
            if (reservation == null)
                return false;

            if (reservation.Status == "Cancelled" || reservation.Status == "CheckedOut")
                return false;

            // Check cancellation policy (24 hours before check-in)
            var hoursUntilCheckIn = (reservation.CheckInDate - DateTime.Now).TotalHours;
            if (hoursUntilCheckIn < 24)
                throw new Exception("Cannot cancel reservation within 24 hours of check-in. Please contact support.");

            // Check if payment was made and refund if needed
            if (reservation.IsPaid)
            {
                // Process refund logic here
                // await _paymentService.RefundPaymentAsync(reservationId, reason);
            }

            reservation.Status = "Cancelled";
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.CancellationReason = reason;
            reservation.UpdatedAt = DateTime.UtcNow;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckAvailabilityAsync(int propertyId, int? roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                return false;

            if (checkIn < DateTime.Now.Date)
                return false;

            if (roomTypeId.HasValue)
            {
                return await IsRoomTypeAvailableAsync(roomTypeId.Value, checkIn, checkOut);
            }

            return await _reservationRepository.IsPropertyAvailableAsync(propertyId, checkIn, checkOut);
        }

        public async Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1)
        {
            return await _reservationRepository.IsRoomTypeAvailableAsync(roomTypeId, checkIn, checkOut, quantity);
        }

        public async Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int propertyId, int? roomTypeId = null)
        {
            return await _reservationRepository.GetAvailableDatesAsync(propertyId, roomTypeId);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reservationRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByPropertyAsync(DateTime startDate, DateTime endDate)
        {
            return await _reservationRepository.GetRevenueByPropertyAsync(startDate, endDate);
        }

        public async Task<int> GetBookingCountByStatusAsync(string status)
        {
            return await _reservationRepository.GetBookingCountByStatusAsync(status);
        }

        public async Task<int> GetTotalGuestsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reservationRepository.GetTotalGuestsAsync(startDate, endDate);
        }

        public async Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reservationRepository.GetAverageDailyRateAsync(startDate, endDate);
        }

        public async Task<double> GetOccupancyRateAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            return await _reservationRepository.GetOccupancyRateAsync(propertyId, startDate, endDate);
        }

        public async Task UpdateReservationStatusAsync(int reservationId, string status)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found");

            reservation.Status = status;
            reservation.UpdatedAt = DateTime.UtcNow;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();
        }

        public async Task<string> GenerateReservationNumberAsync()
        {
            return await _reservationRepository.GenerateReservationNumberAsync();
        }

        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _reservationRepository.CountAsync();
        }

        #region Mapping Methods

        private ReservationViewModel MapToViewModel(Reservation reservation)
        {
            var viewModel = new ReservationViewModel
            {
                Id = reservation.Id,
                ReservationNumber = reservation.ReservationNumber,
                PropertyId = reservation.PropertyId,
                PropertyName = reservation.Property?.Name,
                PropertyImage = reservation.Property?.ImageUrl,
                PropertyType = reservation.Property?.Type,
                PropertyLocation = reservation.Property?.Location,
                RoomTypeId = reservation.RoomTypeId,
                RoomTypeName = reservation.RoomType?.Name,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                NumberOfGuests = reservation.NumberOfGuests,
                NumberOfRooms = reservation.NumberOfRooms,
                TotalPrice = reservation.TotalPrice,
                PricePerNight = reservation.PricePerNight,
                Currency = reservation.Currency,
                Status = reservation.Status,
                SpecialRequests = reservation.SpecialRequests,
                IsPaid = reservation.IsPaid,
                PaymentMethod = reservation.PaymentMethod,
                CreatedAt = reservation.CreatedAt,
                CancelledAt = reservation.CancelledAt,
                CancellationReason = reservation.CancellationReason,
                UserName = reservation.UserName,
                UserEmail = reservation.UserEmail,
                UserPhone = reservation.UserPhone
            };

            return viewModel;
        }

        #endregion
    }
}