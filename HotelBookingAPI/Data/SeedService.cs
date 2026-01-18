using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace HotelBookingAPI.Data
{
    public class SeedService
    {
        private readonly AppDbContext _db;

        public SeedService(AppDbContext db)
        {
            _db = db;
        }

        // Seed sample Hotels and Rooms. If force==true, will run even if data exists.
        public async Task SeedAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            // Ensure migrations applied in case DB is empty
            await _db.Database.MigrateAsync(cancellationToken);

            if (!force && (await _db.Hotels.AnyAsync(cancellationToken)))
            {
                return; // already seeded
            }

            using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Clear relevant tables first if forcing
                if (force)
                {
                    await RemoveExistingData(cancellationToken);
                }

                // Seed room types
                var singleType = new RoomType { Name = "Single", Capacity = 1 };
                var doubleType = new RoomType { Name = "Double", Capacity = 2 };
                var deluxeType = new RoomType { Name = "Deluxe", Capacity = 4 };

                _db.RoomTypes.AddRange(singleType, doubleType, deluxeType);
                await _db.SaveChangesAsync(cancellationToken);

                // Seed hotels
                var hotel1 = new Hotel { Name = "Seaside Inn" };
                var hotel2 = new Hotel { Name = "Mountain Lodge" };
                var hotel3 = new Hotel { Name = "City Center Hotel" };
                var hotel4 = new Hotel { Name = "Airport Plaza" };

                _db.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4);
                await _db.SaveChangesAsync(cancellationToken);

                // Seed rooms
                var rooms = new[]
                {
                    // Seaside Inn - 6 rooms
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel1.Id },
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel1.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel1.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel1.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel1.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel1.Id },
                    
                    // Mountain Lodge - 6 rooms
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel2.Id },
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel2.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel2.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel2.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel2.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel2.Id },
                    
                    // City Center Hotel - 6 rooms
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel3.Id },
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel3.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel3.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel3.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel3.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel3.Id },
                    
                    // Airport Plaza - 6 rooms
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel4.Id },
                    new Room { RoomTypeId = singleType.Id, HotelId = hotel4.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel4.Id },
                    new Room { RoomTypeId = doubleType.Id, HotelId = hotel4.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel4.Id },
                    new Room { RoomTypeId = deluxeType.Id, HotelId = hotel4.Id }
                };
                _db.Rooms.AddRange(rooms);
                await _db.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // Remove all bookings, rooms, hotels (in dependency-safe order)
        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await RemoveExistingData(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task RemoveExistingData(CancellationToken cancellationToken)
        {
            // remove children first to respect FK constraints
            _db.Bookings.RemoveRange(_db.Bookings);
            await _db.SaveChangesAsync(cancellationToken);

            _db.Rooms.RemoveRange(_db.Rooms);
            await _db.SaveChangesAsync(cancellationToken);

            _db.Hotels.RemoveRange(_db.Hotels);
            await _db.SaveChangesAsync(cancellationToken);

            _db.RoomTypes.RemoveRange(_db.RoomTypes);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
