using System;
using System.Collections.Generic;
using System.Threading;

namespace HotelBookingAPI.Services
{
    public interface IRoomService
    {
        Task<List<HotelBookingAPI.Dtos.RoomDto>> GetAvailableForHotelAsync(int hotelId, DateOnly from, DateOnly to, int people, CancellationToken cancellationToken = default);
    }
}