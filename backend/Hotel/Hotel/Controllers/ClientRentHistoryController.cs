using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Hotel.API.Controllers
{
    [Route("api/client/rent-history/")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class ClientRentHistoryController : ControllerBase
    {
        
        private IClientRentHistoryService _rentHistoryService;
        private IClientService _clientService;
        private IClientRoomService _roomService;
        public ClientRentHistoryController(IClientRentHistoryService rentHistoryService,
            IClientService clientService,IClientRoomService roomService)
        {
             _rentHistoryService= rentHistoryService;
            _clientService= clientService;
            _roomService= roomService;
        }
        [HttpPatch("{userId:guid}/bookings/cancel-booking/{bookid:guid}")]
        public async Task<ActionResult> CancelActiveBookingById(Guid userId, Guid bookid)
        {
            try
            {
                await _rentHistoryService.CancelActiveBooking(userId, bookid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id:guid}/bookings")]
        public async Task<ActionResult<List<BookingRentHistoryRepsonseDto>>> GetActiveBookingsAsync(Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            {
                List<Booking> bookings = await _rentHistoryService.GetActiveBookingsAsync(id, page, pageSize);
                List<Room> rooms = await _roomService.GetRoomsForBookingsAsync(bookings);
                return Ok(bookings
                    .Join(rooms, b => b.RoomId, r => r.Id, (booking, room) => (booking, room))
                    .Select(bookingWithDetails => new HistoryBookingResponseDto(bookingWithDetails.booking.Id, bookingWithDetails.room.Title,
                    bookingWithDetails.booking.DateCheckIn, bookingWithDetails.booking.DateCheckOut, bookingWithDetails.booking.ReasonCancel)));
            }
        }
        [HttpGet("{id:guid}/cancelled")]
        public async Task<ActionResult<List<Booking>>> GetCancelledBookingsAsync(Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                List<Booking> bookings = await _rentHistoryService.GetCancelledBookingsAsync(id, page, pageSize);
                List<Room> rooms = await _roomService.GetRoomsForBookingsAsync(bookings);
                return Ok(bookings
                    .Join(rooms, b => b.RoomId, r => r.Id, (booking, room) => (booking, room))
                    .Select(bookingWithDetails => new HistoryBookingResponseDto(bookingWithDetails.booking.Id, bookingWithDetails.room.Title,
                    bookingWithDetails.booking.DateCheckIn, bookingWithDetails.booking.DateCheckOut, bookingWithDetails.booking.ReasonCancel)));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
        [HttpGet("{id:guid}/rents")]
        public async Task<ActionResult<List<RentHistoryResponseDto>>> GetRentsHistoryAsync(Guid id,[FromQuery] int page,[FromQuery] int pageSize)
        {
            try
            {
                List<RentRoom> rents = await _rentHistoryService.GetRentsHistoryAsync(id, page, pageSize);
                List<Booking> bookings = await _rentHistoryService.GetRentsBookingsAsync(rents);
                List<Room> rooms = await _roomService.GetRoomsForBookingsAsync(bookings);

                return Ok(rents
                    .Join(bookings, r => r.BookId, b => b.Id, (rent, booking) => (rent, booking))
                    .Join(rooms, rentWithBooking => rentWithBooking.booking.RoomId, room => room.Id, (rentWithBooking, room) =>
                    (rentWithBooking.rent, rentWithBooking.booking, room))
                    .Select(rentWithDetails =>
                    {
                        return new RentHistoryResponseDto(rentWithDetails.rent.Id, rentWithDetails.room.Title,
                            rentWithDetails.rent.DateCheckIn, rentWithDetails.rent.DateCheckOut.Value, rentWithDetails.rent.Price.Value);
                    })
                    .ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
    
}
