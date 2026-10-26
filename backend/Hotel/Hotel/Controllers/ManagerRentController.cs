using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/manager/rents/")]
    [ApiController]
    [Authorize(Roles ="Manager")]
    public class ManagerRentController : ControllerBase
    {
        private IManagerBookingService _managerBookingService;
        private IManagerRentService _managerRentService;
        private IManagerClientService _managerClientService;
        private IManagerRoomService _managerRoomService;
        public ManagerRentController(IManagerRentService rentService,IManagerBookingService bookingService,
            IManagerClientService clientService,IManagerRoomService roomService)
        {
            _managerBookingService = bookingService;
            _managerRentService = rentService;
            _managerClientService = clientService;
            _managerRoomService = roomService;
        }
        private async Task<Booking> CreateBooking(BookingRequestNewClientDto booking)
        {
            return await _managerBookingService.CreateBookingAsync(booking.PhoneNumber, booking.FirstName,
                booking.LastName, booking.Patronymic, booking.RoomId, DateOnly.FromDateTime(booking.CheckIn), DateOnly.FromDateTime(booking.CheckOut));
        }
        [HttpGet("all")]
        public async Task<ActionResult<List<RentResponseDto>>> GetRents()
        {
            List<Booking> bookings = await _managerBookingService.GetRentedBookingAsync();
            List<RentRoom> rents = await _managerRentService.GetActiveRentsAsync();
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List <Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);
            
            return Ok(rents
                .Join(bookings,rent=>rent.BookId,booking=>booking.Id,(rent,booking)=>(rent,booking))
                .Join(clients,rentWithDetails=>rentWithDetails.booking.ClientId,client=>client.Id,(rentWithDetails,client)=>
                (rentWithDetails.rent,rentWithDetails.booking,client))
                .Join(rooms, rentWithDetails => rentWithDetails.booking.RoomId, room => room.Id, (rentWithDetails, room) =>
                (rentWithDetails.rent, rentWithDetails.booking,rentWithDetails.client, room))
                .Select(rentWithDetails => new RentResponseDto(rentWithDetails.rent.Id, rentWithDetails.rent.BookId,
                rentWithDetails.client.Name.ToString(), rentWithDetails.client.TelephoneNum.Value, rentWithDetails.room.Title,
                    rentWithDetails.rent.DateCheckIn, rentWithDetails.rent.DateCheckOut, rentWithDetails.rent.Price,
                    rentWithDetails.rent.Status.ToString(), rentWithDetails.rent.Note)));
        }
        [HttpGet("bookings")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetBookingForRent()
        {
            List<Booking> bookings = await _managerBookingService.GetTodaysActiveBookingAsync();
            List<Client> clients = await _managerClientService.GetClientsForBookingsAsync(bookings);
            List<Room> rooms = await _managerRoomService.GetRoomsForBookingsAsync(bookings);

            return Ok(bookings
                .Join(clients, booking=> booking.ClientId, client => client.Id, (booking, client) =>
                (booking, client))
                .Join(rooms, bookingWithDetails => bookingWithDetails.booking.RoomId, room => room.Id, (bookingWithDetails, room) =>
                (bookingWithDetails.booking, bookingWithDetails.client, room))
                .Select(bookingWithDetails => new BookingResponseDto(bookingWithDetails.booking.Id, bookingWithDetails.booking.ClientId,
                bookingWithDetails.booking.RoomId, bookingWithDetails.room.Title, bookingWithDetails.client.Name.ToString(),
                bookingWithDetails.client.TelephoneNum.Value, bookingWithDetails.booking.DateCheckIn, bookingWithDetails.booking.DateCheckOut,
                bookingWithDetails.booking.Status.ToString(), bookingWithDetails.booking.Price, bookingWithDetails.booking.ReasonCancel))
                .ToList());
        }
        [HttpPost("bookings/rent/{id:guid}")]
        public async Task<ActionResult> RentRoomByBookingId(Guid id)
        {
            try
            {
                await _managerRentService.RentRoom(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
           
        }
        [HttpPost("bookings/booking-new")]
        public async Task<ActionResult> RentRoomWithoutBooking([FromBody] BookingRequestNewClientDto booking)
        {
            try
            {
                Booking bookingRent =await CreateBooking(booking); 
                await _managerRentService.RentRoom(bookingRent.Id);
                return Ok();
            }
            catch(Exception ex) 
            {
                return BadRequest(new { ex.Message });
            }
            
        }
        [HttpPatch("add-note/{id:guid}")]
        public async Task<ActionResult> AddNoteAsync(Guid id, [FromBody]string note)
        {
            try
            {
                await _managerRentService.AddNoteAsync(id, note);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("fix-checkout/{id:guid}")]
        public async Task<ActionResult> FixCheckout(Guid id)
        {
            try
            {
                await _managerRentService.FixCheckout(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
