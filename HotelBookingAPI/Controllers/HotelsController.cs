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

        // GET api/hotels?name=inn
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, CancellationToken cancellationToken = default)
        {
            var hotels = await _hotelService.SearchAsync(name, cancellationToken);
            return Ok(hotels);
        }
    }
}