using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.AdministratorServices
{
    public interface IAdminRoomService
    {

        Task<List<Room>> GetRoomsAsync();
        Task<Guid> CreateRoom(string title, int count, string description, decimal price);
        Task UpdateRoom(Guid roomId,string title, string description, decimal price, RoomStatus status, int count);
        Task DeleteRoom(Guid roomId);
    }
}
