using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;

        public RoomService(AppDbContext db) => _db = db;

        public async Task<List<RoomDto>> GetAvailableForHotelAsync(int hotelId,
            DateOnly from,
            DateOnly to,
            int people,
            CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (from >= to) throw new ArgumentException("'from' must be before 'to'");
            if (people <= 0) throw new ArgumentException("'people' must be > 0");

            // Check hotel existence
            var hotelExists = await _db.Hotels.AnyAsync(h => h.Id == hotelId, cancellationToken);
            if (!hotelExists) throw new KeyNotFoundException("Hotel not found");

            var roomsQuery = _db.Rooms.AsQueryable();
            roomsQuery = roomsQuery
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotelId && r.RoomType.Capacity >= people);

            var available = await roomsQuery
                .Where(r => !_db.Bookings.Any(b =>
                    b.RoomID == r.Id &&
                    b.CheckIn < to &&
                    b.CheckOut > from))
                .Select(r => new RoomDto(r.Id, r.RoomTypeId, r.RoomType.Name, r.RoomType.Capacity, r.HotelId))
                .ToListAsync(cancellationToken);

            return available;
        }
    }
}