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

        /// <summary>
        /// Gets available rooms for a hotel based on dates and number of people.
        /// </summary>
        /// <param name="hotelId">The hotel ID.</param>
        /// <param name="from">Check-in date.</param>
        /// <param name="to">Check-out date.</param>
        /// <param name="people">Number of people (1-4).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of available rooms with pricing.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/hotels/1/rooms?from=2026-01-20&amp;to=2026-01-22&amp;people=2
        ///     
        /// </remarks>
        /// <response code="200">Returns available rooms.</response>
        /// <response code="400">Invalid request parameters.</response>
        /// <response code="404">Hotel not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<HotelBookingAPI.Dtos.RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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