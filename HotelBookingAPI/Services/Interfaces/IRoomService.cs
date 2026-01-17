using System;
using System.Collections.Generic;
using System.Threading;

namespace HotelBookingAPI.Services
{
    public interface IRoomService
    {
        //Task<List<HotelBookingAPI.Dtos.RoomDto>> GetAvailableAsync(int? hotelId, DateTime from, DateTime to, int people, CancellationToken cancellationToken = default);

        Task<List<HotelBookingAPI.Dtos.RoomDto>> GetAvailableForHotelAsync(int hotelId, DateTime from, DateTime to, int people, CancellationToken cancellationToken = default);
    }
}