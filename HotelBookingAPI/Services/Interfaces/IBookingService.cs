using System.Threading;

namespace HotelBookingAPI.Services
{
    public interface IBookingService
    {
        Task<(HotelBookingAPI.Dtos.BookingCreatedDto? Created, int StatusCode, string? Error)> CreateAsync(HotelBookingAPI.Dtos.CreateBookingDto request, CancellationToken cancellationToken = default);

        Task<HotelBookingAPI.Dtos.BookingDetailsDto?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    }
}