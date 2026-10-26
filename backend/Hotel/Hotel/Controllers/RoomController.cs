using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hotel.API.Controllers
{
    [Route("api/rooms/")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IImageService _imageService;
        private IClientRoomService _clientRoomService;
        private IManagerRoomService _managerRoomService;
        public RoomController(IImageService imageService,IClientRoomService roomService,IManagerRoomService managerRoomService)
        {
            _imageService=imageService;
            _clientRoomService=roomService; 
            _managerRoomService=managerRoomService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<RoomResponseDto>>> GetRooms([FromQuery] int page, [FromQuery] int pageSize)
        {
            List<Room> rooms= await _clientRoomService.GetRoomsByPageAsync(page, pageSize);
            return rooms.Select(r => new RoomResponseDto(r.Id   ,r.Title,r.Description,r.CountPerson,r.Price))
                .ToList();
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RoomResponseDto>> GetRooms(Guid id)
        {
            try
            {
                Room room = await _clientRoomService.GetRoomById(id);
                return Ok(new RoomResponseDto(room.Id, room.Title, room.Description, room.CountPerson, room.Price));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
       
        }
        [HttpGet("count")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCountRooms()
        {
            return await _clientRoomService.GetCountRooms();
        }
        [HttpGet("room-{id:guid}/image")]
        [AllowAnonymous]
        public async Task<ActionResult> GetImage(Guid id)           
        {
           try
           { 
                FileStream stream =await _imageService.GetImageStream(id);
                string content = await _imageService.GetContentType(id);
                return File(stream, content);
           }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpGet("staff/all-rooms")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<List<RoomStaffResponseDto>>> GetAllRooms()
        {
            List<Room> rooms = await _managerRoomService.GetRoomsAsync();
            return rooms.Select(r => new RoomStaffResponseDto(r.Id, r.Title, 
                r.CountPerson,r.Description, r.Price,r.Status.ToString()))
                .ToList();
        }
    }
}
