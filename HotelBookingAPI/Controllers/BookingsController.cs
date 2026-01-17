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

        // POST api/bookings
        [HttpPost]
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

        // GET api/bookings/{reference}
        [HttpGet("{reference}")]
        public async Task<IActionResult> GetByReference(string reference, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference)) return BadRequest();

            var details = await _bookingService.GetByReferenceAsync(reference, cancellationToken);
            if (details == null) return NotFound();

            return Ok(details);
        }
    }
}