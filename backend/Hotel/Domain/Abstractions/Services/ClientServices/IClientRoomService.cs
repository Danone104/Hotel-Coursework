using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ClientServices
{
    public interface IClientRoomService
    {
        Task<List<Room>> GetRoomsByPageAsync(int page, int pageSize);
        Task<Room> GetRoomById(Guid id);
        Task<List<Room>> GetRoomsForBookingsAsync(List<Booking> bookings);
        Task<int> GetCountRooms();
    }
}
