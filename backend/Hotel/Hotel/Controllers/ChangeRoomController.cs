using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ChangeRoomController : ControllerBase
    {
        private IAdminRoomService _adminRoomService;
        private IAdminImageService _adminImageService;
        public ChangeRoomController(IAdminRoomService roomService,IAdminImageService imageService)
        {
            _adminImageService = imageService;
            _adminRoomService = roomService;
        }

        [HttpPost("add-room")]
        public async Task<ActionResult> AddRoom([FromForm] RoomAddRequest roomRequest)
        {
            try
            {
                Guid roomId= await _adminRoomService.CreateRoom(roomRequest.Title, 
                    roomRequest.Count,roomRequest.Description,roomRequest.Price);
                await _adminImageService.UploadImageAsync(roomId,roomRequest.Image);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
        [HttpPatch("update-room/{id:guid}")]
        public async Task<ActionResult> UpdateRoom(Guid id, [FromForm] RoomAddRequest roomRequest)
        {
            try
            {
                Enum.TryParse(roomRequest.Status,out RoomStatus status);
                await _adminRoomService.UpdateRoom(id,roomRequest.Title,
                 roomRequest.Description,  roomRequest.Price, status,roomRequest.Count);
                if(roomRequest.Image!=null) await _adminImageService.UploadImageAsync(id, roomRequest.Image);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete-room/{id:guid}")]
        public async Task<ActionResult> DeleteRoom(Guid id)
        {
            try
            {
                await _adminRoomService.DeleteRoom(id);
                await _adminImageService.DeleteImageAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
