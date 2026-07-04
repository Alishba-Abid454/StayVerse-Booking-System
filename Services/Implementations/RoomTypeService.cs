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
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomTypeService(
            IRoomTypeRepository roomTypeRepository,
            IReservationRepository reservationRepository)
        {
            _roomTypeRepository = roomTypeRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByPropertyAsync(int propertyId)
        {
            var roomTypes = await _roomTypeRepository.GetRoomTypesByPropertyAsync(propertyId);
            return roomTypes.Select(r => MapToViewModel(r));
        }

        public async Task<RoomTypeViewModel> GetRoomTypeByIdAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            return roomType != null ? MapToViewModel(roomType) : null;
        }

        public async Task<RoomTypeViewModel> GetRoomTypeWithDetailsAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetRoomTypeWithDetailsAsync(id);
            return roomType != null ? MapToViewModel(roomType) : null;
        }

        public async Task<IEnumerable<RoomTypeViewModel>> GetAvailableRoomTypesAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var roomTypes = await _roomTypeRepository.GetAvailableRoomTypesAsync(propertyId, checkIn, checkOut);
            return roomTypes.Select(r => MapToViewModel(r));
        }

        public async Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByCapacityAsync(int minGuests, int maxGuests)
        {
            var roomTypes = await _roomTypeRepository.GetRoomTypesByCapacityAsync(minGuests, maxGuests);
            return roomTypes.Select(r => MapToViewModel(r));
        }

        public async Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1)
        {
            return await _roomTypeRepository.IsRoomTypeAvailableAsync(roomTypeId, checkIn, checkOut, quantity);
        }

        public async Task<int> GetAvailableQuantityAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            return await _roomTypeRepository.GetAvailableQuantityAsync(roomTypeId, checkIn, checkOut);
        }

        public async Task<IEnumerable<DateTime>> GetBookedDatesAsync(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            return await _roomTypeRepository.GetBookedDatesAsync(roomTypeId, startDate, endDate);
        }

        public async Task<RoomTypeViewModel> CreateRoomTypeAsync(RoomTypeViewModel model)
        {
            var roomType = new RoomType
            {
                PropertyId = model.PropertyId,
                Name = model.Name,
                Description = model.Description,
                BedType = model.BedType,
                MaxGuests = model.MaxGuests,
                Bedrooms = model.Bedrooms > 0 ? model.Bedrooms : 1,
                Bathrooms = model.Bathrooms > 0 ? model.Bathrooms : 1,
                PricePerNight = model.PricePerNight,
                WeekendPrice = model.WeekendPrice > 0 ? model.WeekendPrice : model.PricePerNight,
                Quantity = model.Quantity > 0 ? model.Quantity : 1,
                AvailableQuantity = model.Quantity > 0 ? model.Quantity : 1,
                Size = model.Size,
                View = model.View,
                IsSmokingAllowed = model.IsSmokingAllowed,
                IsPetFriendly = model.IsPetFriendly,
                RoomAmenities = model.RoomAmenities ?? new List<string>(),
                IsActive = true
            };

            await _roomTypeRepository.AddAsync(roomType);
            await _roomTypeRepository.SaveChangesAsync();

            return MapToViewModel(roomType);
        }

        public async Task UpdateRoomTypeAsync(RoomTypeViewModel model)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(model.Id);
            if (roomType == null)
                throw new Exception("Room type not found");

            roomType.Name = model.Name;
            roomType.Description = model.Description;
            roomType.BedType = model.BedType;
            roomType.MaxGuests = model.MaxGuests;
            roomType.Bedrooms = model.Bedrooms;
            roomType.Bathrooms = model.Bathrooms;
            roomType.PricePerNight = model.PricePerNight;
            roomType.WeekendPrice = model.WeekendPrice;
            roomType.Quantity = model.Quantity;
            roomType.AvailableQuantity = model.AvailableQuantity;
            roomType.Size = model.Size;
            roomType.View = model.View;
            roomType.IsSmokingAllowed = model.IsSmokingAllowed;
            roomType.IsPetFriendly = model.IsPetFriendly;
            roomType.RoomAmenities = model.RoomAmenities;

            _roomTypeRepository.Update(roomType);
            await _roomTypeRepository.SaveChangesAsync();
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType != null)
            {
                roomType.IsActive = false;
                _roomTypeRepository.Update(roomType);
                await _roomTypeRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateAvailabilityAsync(int roomTypeId, int quantity)
        {
            await _roomTypeRepository.UpdateAvailabilityAsync(roomTypeId, quantity);
        }

        public async Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var roomTypes = await _roomTypeRepository.GetRoomTypesByPriceRangeAsync(minPrice, maxPrice);
            return roomTypes.Select(r => MapToViewModel(r));
        }

        public async Task<decimal> GetAveragePriceByPropertyAsync(int propertyId)
        {
            return await _roomTypeRepository.GetAveragePriceByPropertyAsync(propertyId);
        }

        public async Task<IEnumerable<RoomTypeViewModel>> GetMostBookedRoomTypesAsync(int propertyId, int count = 5)
        {
            var roomTypes = await _roomTypeRepository.GetMostBookedRoomTypesAsync(propertyId, count);
            return roomTypes.Select(r => MapToViewModel(r));
        }

        #region Mapping Methods

        private RoomTypeViewModel MapToViewModel(RoomType roomType)
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
                IsAvailable = roomType.AvailableQuantity > 0,
                Images = new List<string>()
            };
        }

        #endregion
    }
}