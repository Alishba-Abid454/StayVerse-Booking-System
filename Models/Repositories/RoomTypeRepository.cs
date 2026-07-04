using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RoomType>> GetRoomTypesByPropertyAsync(int propertyId)
        {
            return await _context.RoomTypes
                .Where(r => r.PropertyId == propertyId && r.IsActive)
                .OrderBy(r => r.PricePerNight)
                .ToListAsync();
        }

        public async Task<RoomType> GetRoomTypeWithDetailsAsync(int id)
        {
            return await _context.RoomTypes
                .Include(r => r.Property)
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RoomType>> GetAvailableRoomTypesAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var roomTypes = await _context.RoomTypes
                .Where(r => r.PropertyId == propertyId && r.IsActive)
                .ToListAsync();

            var availableRoomTypes = new List<RoomType>();

            foreach (var roomType in roomTypes)
            {
                if (await IsRoomTypeAvailableAsync(roomType.Id, checkIn, checkOut))
                {
                    availableRoomTypes.Add(roomType);
                }
            }

            return availableRoomTypes.OrderBy(r => r.PricePerNight);
        }

        public async Task<IEnumerable<RoomType>> GetRoomTypesByCapacityAsync(int minGuests, int maxGuests)
        {
            return await _context.RoomTypes
                .Include(r => r.Property)
                .Where(r => r.MaxGuests >= minGuests && r.MaxGuests <= maxGuests && r.IsActive)
                .OrderBy(r => r.PricePerNight)
                .ToListAsync();
        }

        public async Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1)
        {
            var overlapping = await _context.Reservations
                .Where(r => r.RoomTypeId == roomTypeId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn)
                .SumAsync(r => r.NumberOfRooms);

            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType == null)
                return false;

            return (overlapping + quantity) <= roomType.AvailableQuantity;
        }

        public async Task<int> GetAvailableQuantityAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var overlapping = await _context.Reservations
                .Where(r => r.RoomTypeId == roomTypeId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn)
                .SumAsync(r => r.NumberOfRooms);

            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType == null)
                return 0;

            return roomType.AvailableQuantity - overlapping;
        }

        public async Task<IEnumerable<DateTime>> GetBookedDatesAsync(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            var reservations = await _context.Reservations
                .Where(r => r.RoomTypeId == roomTypeId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < endDate &&
                           r.CheckOutDate > startDate)
                .ToListAsync();

            var bookedDates = new List<DateTime>();

            foreach (var reservation in reservations)
            {
                for (var date = reservation.CheckInDate; date < reservation.CheckOutDate; date = date.AddDays(1))
                {
                    if (date >= startDate && date <= endDate)
                    {
                        bookedDates.Add(date);
                    }
                }
            }

            return bookedDates.Distinct().OrderBy(d => d);
        }

        public async Task UpdateAvailabilityAsync(int roomTypeId, int quantity)
        {
            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType != null)
            {
                roomType.AvailableQuantity = quantity;
                _context.RoomTypes.Update(roomType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RoomType>> GetRoomTypesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.RoomTypes
                .Include(r => r.Property)
                .Where(r => r.PricePerNight >= minPrice && r.PricePerNight <= maxPrice && r.IsActive)
                .OrderBy(r => r.PricePerNight)
                .ToListAsync();
        }

        public async Task<decimal> GetAveragePriceByPropertyAsync(int propertyId)
        {
            var roomTypes = await _context.RoomTypes
                .Where(r => r.PropertyId == propertyId && r.IsActive)
                .ToListAsync();

            if (!roomTypes.Any())
                return 0;

            return roomTypes.Average(r => r.PricePerNight);
        }

        public async Task<IEnumerable<RoomType>> GetMostBookedRoomTypesAsync(int propertyId, int count = 5)
        {
            var roomTypeBookings = await _context.Reservations
                .Where(r => r.PropertyId == propertyId &&
                           r.Status != "Cancelled" &&
                           r.RoomTypeId.HasValue)
                .GroupBy(r => r.RoomTypeId)
                .Select(g => new { RoomTypeId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToListAsync();

            var roomTypeIds = roomTypeBookings.Select(x => x.RoomTypeId.Value).ToList();
            return await _context.RoomTypes
                .Where(r => roomTypeIds.Contains(r.Id))
                .ToListAsync();
        }
    }
}
