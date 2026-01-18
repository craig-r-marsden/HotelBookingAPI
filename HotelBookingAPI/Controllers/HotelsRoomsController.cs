using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelid:int}/rooms")]
    public class HotelsRoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public HotelsRoomsController(IRoomService roomService) => _roomService = roomService;

        // GET api/hotels/{hotelid}/rooms?from=2026-01-20&to=2026-01-22&people=2
        [HttpGet]
        public async Task<IActionResult> GetAvailable(
            [FromRoute(Name = "hotelid")] int hotelId,
            [FromQuery][Required] DateOnly from,
            [FromQuery][Required] DateOnly to,
            [FromQuery][Range(1, 4)] int people = 1,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var available = await _roomService.GetAvailableForHotelAsync(hotelId, from, to, people, cancellationToken);
                return Ok(available);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}