using Hotel.Domain.Models;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IRentRepository
    {
        Task AddRentAsync(RentRoom rent);
        Task<List<RentRoom>> GetActiveRentAsync();
        Task<RentRoom?> GetRentByIdAsync(Guid id);
        Task<List<RentRoom>> GetRentsByBookingsAsync(List<Booking> bookings);
        Task SaveChangesAsync();
    }
}