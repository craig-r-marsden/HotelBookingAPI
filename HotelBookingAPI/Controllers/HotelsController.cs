using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Services;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService) => _hotelService = hotelService;

        /// <summary>
        /// Searches for hotels by name.
        /// </summary>
        /// <param name="name">Optional hotel name search filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of hotels matching the search criteria.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/hotels?name=Lodge
        ///     
        /// </remarks>
        /// <response code="200">Returns list of hotels.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HotelBookingAPI.Dtos.HotelDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string? name, CancellationToken cancellationToken = default)
        {
            var hotels = await _hotelService.SearchAsync(name, cancellationToken);
            return Ok(hotels);
        }
    }
}