using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Services;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="request">The booking details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created booking with reference number.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/bookings
        ///     {
        ///        "roomId": 1,
        ///        "guestName": "John Smith",
        ///        "checkIn": "2026-01-20",
        ///        "checkOut": "2026-01-22",
        ///        "numberOfGuests": 2
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Booking created successfully.</response>
        /// <response code="400">Invalid booking request.</response>
        /// <response code="404">Room or hotel not found.</response>
        /// <response code="409">Room not available for selected dates.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BookingCreatedDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto request, CancellationToken cancellationToken = default)
        {
            var (created, status, error) = await _bookingService.CreateAsync(request, cancellationToken);

            if (!string.IsNullOrEmpty(error))
            {
                return status switch
                {
                    400 => BadRequest(new { error }),
                    404 => NotFound(new { error }),
                    409 => Conflict(new { error }),
                    _ => Problem(detail: error, statusCode: status)
                };
            }

            return CreatedAtAction(nameof(GetByReference), new { reference = created!.Reference }, created);
        }

        /// <summary>
        /// Gets booking details by reference number.
        /// </summary>
        /// <param name="reference">The booking reference number.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The booking details.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/bookings/84E58BFF
        ///     
        /// </remarks>
        /// <response code="200">Returns the booking details.</response>
        /// <response code="400">Invalid reference.</response>
        /// <response code="404">Booking not found.</response>
        [HttpGet("{reference}")]
        [ProducesResponseType(typeof(BookingDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByReference(string reference, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference)) return BadRequest();

            var details = await _bookingService.GetByReferenceAsync(reference, cancellationToken);
            if (details == null) return NotFound();

            return Ok(details);
        }
    }
}