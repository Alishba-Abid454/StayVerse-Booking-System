using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 6)
        {
            return await _context.Properties
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByTypeAsync(string type)
        {
            return await _context.Properties
                .Where(p => p.Type == type && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city)
        {
            return await _context.Properties
                .Where(p => p.City == city && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string location, DateTime? checkIn, DateTime? checkOut, int? guests, string type)
        {
            var query = _context.Properties.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(p =>
                    p.Location.Contains(location) ||
                    p.City.Contains(location) ||
                    p.Country.Contains(location));
            }

            if (!string.IsNullOrEmpty(type) && type != "All")
            {
                query = query.Where(p => p.Type == type);
            }

            if (guests.HasValue)
            {
                query = query.Where(p => p.MaxGuests >= guests.Value);
            }

            if (checkIn.HasValue && checkOut.HasValue)
            {
                var properties = await query.ToListAsync();
                var availableProperties = new List<Property>();

                foreach (var property in properties)
                {
                    if (await IsPropertyAvailableAsync(property.Id, checkIn.Value, checkOut.Value))
                    {
                        availableProperties.Add(property);
                    }
                }

                return availableProperties.OrderByDescending(p => p.Rating);
            }

            return await query.OrderByDescending(p => p.Rating).ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesWithAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests)
        {
            var properties = await _context.Properties
                .Include(p => p.RoomTypes)
                .Where(p => p.IsActive)
                .ToListAsync();

            var availableProperties = new List<Property>();

            foreach (var property in properties)
            {
                var availableRooms = property.RoomTypes
                    .Where(r => r.MaxGuests >= guests && r.IsActive)
                    .ToList();

                if (availableRooms.Any())
                {
                    foreach (var room in availableRooms)
                    {
                        if (await IsRoomTypeAvailableAsync(room.Id, checkIn, checkOut))
                        {
                            availableProperties.Add(property);
                            break;
                        }
                    }
                }
            }

            return availableProperties.OrderByDescending(p => p.Rating);
        }

        public async Task<Property> GetPropertyWithDetailsAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Amenities)
                .Include(p => p.RoomTypes)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetPropertyWithRoomsAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.RoomTypes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetPropertyWithReviewsAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<double> GetAverageRatingAsync(int propertyId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<int> GetTotalReviewsAsync(int propertyId)
        {
            return await _context.Reviews
                .CountAsync(r => r.PropertyId == propertyId);
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var overlappingReservations = await _context.Reservations
                .Where(r => r.PropertyId == propertyId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn)
                .CountAsync();

            return overlappingReservations == 0;
        }

        private async Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var overlappingReservations = await _context.Reservations
                .Where(r => r.RoomTypeId == roomTypeId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn)
                .CountAsync();

            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType == null)
                return false;

            return overlappingReservations < roomType.AvailableQuantity;
        }

        public async Task<IEnumerable<Property>> GetTopRatedPropertiesAsync(int count = 10)
        {
            return await _context.Properties
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetMostBookedPropertiesAsync(int count = 10)
        {
            var propertyBookings = await _context.Reservations
                .Where(r => r.Status != "Cancelled")
                .GroupBy(r => r.PropertyId)
                .Select(g => new { PropertyId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToListAsync();

            var propertyIds = propertyBookings.Select(x => x.PropertyId).ToList();
            return await _context.Properties
                .Where(p => propertyIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Dictionary<int, double>> GetOccupancyRatesAsync(DateTime startDate, DateTime endDate)
        {
            var properties = await _context.Properties
                .Where(p => p.IsActive)
                .ToListAsync();

            var occupancyRates = new Dictionary<int, double>();

            foreach (var property in properties)
            {
                var totalNights = (endDate - startDate).Days;

                // Get all reservations for this property in the date range
                var reservations = await _context.Reservations
                    .Where(r => r.PropertyId == property.Id &&
                               r.Status != "Cancelled" &&
                               r.CheckInDate < endDate &&
                               r.CheckOutDate > startDate)
                    .ToListAsync();

                // Calculate booked nights using client evaluation
                var bookedNights = reservations.Sum(r => (r.CheckOutDate - r.CheckInDate).Days);

                var occupancyRate = totalNights > 0 ? (double)bookedNights / totalNights * 100 : 0;
                occupancyRates[property.Id] = Math.Round(occupancyRate, 2);
            }

            return occupancyRates;
        }

        public async Task<IEnumerable<Property>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Properties
                .Where(p => p.BasePrice >= minPrice && p.BasePrice <= maxPrice && p.IsActive)
                .OrderBy(p => p.BasePrice)
                .ToListAsync();
        }
    }
}
