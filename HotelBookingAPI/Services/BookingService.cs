using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext db, ILogger<BookingService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(BookingCreatedDto? Created, int StatusCode, string? Error)> CreateAsync(CreateBookingDto request, CancellationToken cancellationToken = default)
        {
            if (request.CheckIn >= request.CheckOut)
                return (null, 400, "CheckIn must be before CheckOut");

            if (request.NumberOfGuests <= 0)
                return (null, 400, "NumberOfGuests must be > 0");

            if (string.IsNullOrWhiteSpace(request.GuestName))
                return (null, 400, "GuestName is required");

            var room = await _db.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
            if (room == null) return (null, 404, "Room not found");

            if (room.RoomType.Capacity < request.NumberOfGuests) return (null, 400, "Room capacity is insufficient");

            var overlap = await _db.Bookings.AnyAsync(b =>
                b.RoomID == room.Id &&
                b.CheckIn < request.CheckOut &&
                b.CheckOut > request.CheckIn, cancellationToken);

            if (overlap) return (null, 409, "Room is not available for the requested dates");

            // Generate a short unique booking reference
            string reference;
            do
            {
                reference = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
            } while (await _db.Bookings.AnyAsync(b => b.Reference == reference, cancellationToken));

            var booking = new Booking
            {
                Reference = reference,
                GuestName = request.GuestName,
                RoomID = room.Id,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                NumberOfGuests = request.NumberOfGuests
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created booking {Reference} for room {RoomId}", reference, room.Id);

            return (new BookingCreatedDto(booking.Id, booking.Reference), 201, null);
        }

        public async Task<BookingDetailsDto?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference)) return null;

            var booking = await _db.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Reference == reference, cancellationToken);

            if (booking == null) return null;

            var room = await _db.Rooms
                .Include(r => r.RoomType)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == booking.RoomID, cancellationToken);
            HotelDto? hotelDto = null;
            RoomDto? roomDto = null;

            if (room != null)
            {
                roomDto = new RoomDto(room.Id,
                    room.RoomTypeId,
                    room.RoomType.Name,
                    room.RoomType.Capacity,
                    room.HotelId);

                var hotel = await _db.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.Id == room.HotelId, cancellationToken);
                if (hotel != null) hotelDto = new HotelDto(hotel.Id, hotel.Name);
            }

            return new BookingDetailsDto(
                booking.Id,
                booking.Reference,
                booking.GuestName,
                booking.CheckIn,
                booking.CheckOut,
                booking.NumberOfGuests,
                roomDto,
                hotelDto
            );
        }
    }
}