using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotel.API.Controllers
{
    [Route("api/manager/bookings/")]
    [ApiController]
    [Authorize(Roles ="Manager")]
    public class ManagerBookingController : ControllerBase
    {
        private IManagerBookingService _managerBookingService;
        private IManagerClientService _managerClientService;
        private IManagerRoomService _managerRoomService;
        public ManagerBookingController(IManagerBookingService managerBookingService,IManagerClientService clientService,
            IManagerRoomService roomService)
        {
            _managerBookingService = managerBookingService;
            _managerClientService = clientService;
            _managerRoomService= roomService;  

        }

        [HttpGet("today")] 
        public async Task<ActionResult<List<BookingResponseDto>>> GetTodaysBookings([FromQuery] bool withCancelled)
        {
            List<Booking> bookings = withCancelled ? await _managerBookingService.GetTodaysActiveAndCancelledAsync() :
                await _managerBookingService.GetTodaysActiveBookingAsync();
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List<Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);
            return Ok(MapToDto(bookings,clients,rooms));
        }
        private List<BookingResponseDto> MapToDto(List<Booking> bookings, List<Client>clients,List<Room> rooms)
        {
            return bookings
                .Join(clients, booking => booking.ClientId, client => client.Id, (booking, client) => (booking, client))
                .Join(rooms, bookingWithClient => bookingWithClient.booking.RoomId, room => room.Id, (bookingWithClient, room) =>
                (bookingWithClient.booking, bookingWithClient.client, room))
                .Select(bookingWithDetails => new BookingResponseDto(bookingWithDetails.booking.Id, bookingWithDetails.booking.ClientId, 
                bookingWithDetails.booking.RoomId,bookingWithDetails.room.Title, bookingWithDetails.client.Name.ToString(),
                bookingWithDetails.client.TelephoneNum.Value, bookingWithDetails.booking.DateCheckIn, 
                bookingWithDetails.booking.DateCheckOut, bookingWithDetails.booking.Status.ToString(),
                bookingWithDetails.booking.Price, bookingWithDetails.booking.ReasonCancel))
                .ToList();
        }
        
        [HttpGet("date/{date}")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetBookingsByDate(DateOnly date, [FromQuery] bool withCancelled)
        {
            List<Booking> bookings = withCancelled ? await _managerBookingService.GetActiveAndCancelledForDateAsync(date) :
                await _managerBookingService.GetActiveBookingForDateAsync(date);
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List<Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);
            return Ok(MapToDto(bookings, clients, rooms));
        }
        [HttpGet("rented")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetBookingsRented()
        {
            List<Booking> bookings = await _managerBookingService.GetRentedBookingAsync();
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List<Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);
            return Ok(MapToDto(bookings, clients, rooms));
        }
        [HttpGet("all")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetAllActiveBookings()
        {
            List<Booking> bookings = await _managerBookingService.GetActiveBookingsAsync();
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List<Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);
            return Ok(MapToDto(bookings, clients, rooms));
        }
        
        [HttpPatch("{id:guid}/cancel")]
        public async Task<ActionResult> CancelBooking(Guid id,[FromBody] string reason)
        {
            try
            {
                await _managerBookingService.CancelBookAsync(id, reason);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPatch("{id:guid}/reshedule")]
        public async Task<ActionResult> ResheduleBooking(Guid id, [FromBody] DateOnly dateCheckIn)
        {
            try
            {
                await _managerBookingService.ResheduleBookingAsync(id,dateCheckIn);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPatch("{id:guid}/extend")]
        public async Task<ActionResult> ExtendBooking(Guid id, [FromBody] DateOnly dateCheckOut)
        {
            try
            {
                await _managerBookingService.ExtendBookingAsync(id,dateCheckOut);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
    }
}
