using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Services
{
    public class HotelService : IHotelService
    {
        private readonly AppDbContext _db;

        public HotelService(AppDbContext db) => _db = db;

        public async Task<List<HotelDto>> SearchAsync(string? name, CancellationToken cancellationToken = default)
        {
            var query = _db.Hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(h => EF.Functions.Like(h.Name, $"%{name}%"));
            }

            return await query.Select(h => new HotelDto(h.Id, h.Name)).ToListAsync(cancellationToken);
        }
    }
}