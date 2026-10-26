using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ManagerServices
{
    public interface IManagerRoomService
    {
        Task<List<Room>> GetRoomsAsync();
        Task<Room> GetRoomById(Guid id);
        Task<List<Room>> GetRoomsForBookingsAsync(List<Booking> bookings);
    }
}
