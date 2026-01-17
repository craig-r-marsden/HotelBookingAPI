using System.Collections.Generic;
using System.Threading;

namespace HotelBookingAPI.Services
{
    public interface IHotelService
    {
        Task<List<HotelBookingAPI.Dtos.HotelDto>> SearchAsync(string? name, CancellationToken cancellationToken = default);
    }
}