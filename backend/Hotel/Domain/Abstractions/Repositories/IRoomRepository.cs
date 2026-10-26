using Hotel.Domain.Models;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IRoomRepository
    {
        Task AddRoom(Room room);
        Task<Room?> GetRoomById(Guid id);
        Task<List<Room>> GetRooms();
        Task<List<Room>> GetRoomsByPage(int page, int pageSize);
        Task<int> GetCountRooms();
        Task<List<Room>> GetRoomsByBookingsAsync(List<Booking> bookings);
        Task SaveChangesAsync();
    }
}