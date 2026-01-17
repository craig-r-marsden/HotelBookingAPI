using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;

        public RoomService(AppDbContext db) => _db = db;

        //public async Task<List<RoomDto>> GetAvailableAsync(int? hotelId, DateTime from, DateTime to, int people, CancellationToken cancellationToken = default)
        //{
        //    if (from >= to) throw new ArgumentException("from must be before to");
        //    if (people <= 0) throw new ArgumentException("people must be > 0");

        //    var roomsQuery = _db.Rooms.AsQueryable();
        //    if (hotelId.HasValue) roomsQuery = roomsQuery.Where(r => r.HotelId == hotelId.Value);
        //    roomsQuery = roomsQuery.Where(r => r.Capacity >= people);

        //    var available = await roomsQuery
        //        .Where(r => !_db.Bookings.Any(b =>
        //            b.RoomID == r.Id &&
        //            b.CheckIn < to &&
        //            b.CheckOut > from))
        //        .Select(r => new RoomDto(r.Id, r.Type, r.Capacity, r.HotelId))
        //        .ToListAsync(cancellationToken);

        //    return available;
        //}

        //public async Task<List<RoomDto>> GetAvailableForHotelAsync(int hotelId, DateTime from, DateTime to, int people, CancellationToken cancellationToken = default)
        //{
        //    // Validate inputs
        //    if (from >= to) throw new ArgumentException("from must be before to");
        //    if (people <= 0) throw new ArgumentException("people must be > 0");

        //    // Check hotel existence
        //    var hotelExists = await _db.Hotels.AnyAsync(h => h.Id == hotelId, cancellationToken);
        //    if (!hotelExists) throw new KeyNotFoundException("Hotel not found");

        //    // Reuse existing logic with hotel filter
        //    return await GetAvailableAsync(hotelId, from, to, people, cancellationToken);
        //}

        public async Task<List<RoomDto>> GetAvailableForHotelAsync(int hotelId,
            DateTime from,
            DateTime to,
            int people,
            CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (from.Date >= to.Date) throw new ArgumentException("'from' must be before 'to'");
            if (people <= 0) throw new ArgumentException("'people' must be > 0");

            // Check hotel existence
            var hotelExists = await _db.Hotels.AnyAsync(h => h.Id == hotelId, cancellationToken);
            if (!hotelExists) throw new KeyNotFoundException("Hotel not found");

            var roomsQuery = _db.Rooms.AsQueryable();
            roomsQuery = roomsQuery.Where(r => r.HotelId == hotelId && r.Capacity >= people);
            //roomsQuery = roomsQuery.Where(r => r.Capacity >= people);

            var available = await roomsQuery
                .Where(r => !_db.Bookings.Any(b =>
                    b.RoomID == r.Id &&
                    b.CheckIn < to &&
                    b.CheckOut > from))
                .Select(r => new RoomDto(r.Id, r.Type, r.Capacity, r.HotelId))
                .ToListAsync(cancellationToken);

            return available;
        }
    }
}