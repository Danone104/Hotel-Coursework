using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/public/booking/")]
    [ApiController]
    [AllowAnonymous]
    public class PublicBookingController : ControllerBase
    {
        IClientBookingService _clientBookingService;
        public PublicBookingController(IClientBookingService bookingService)
        {
            _clientBookingService = bookingService;
        }

        [HttpGet("occupied-dates/{roomId:guid}")]
        public async Task<ActionResult<List<DateOnly>>> GetOccupiedDates(Guid roomId,[FromQuery] int year, [FromQuery]int month)
        {
            try
            {
                return Ok(await _clientBookingService.GetOccupiedDatesByMonthAsync(roomId, year, month));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<ActionResult> BookRoom([FromBody] BookingRequestNewClientDto booking)
        {
            try
            {
                await _clientBookingService.CreateBookingWithoutAuthAsync(booking.PhoneNumber, booking.FirstName, booking.LastName,
                    booking.Patronymic, booking.RoomId, DateOnly.FromDateTime(booking.CheckIn), DateOnly.FromDateTime(booking.CheckOut));
                return Ok();
            }
            catch(Exception ex) 
            {
                return BadRequest(new { ex.Message });
            }
        }
    }

}
