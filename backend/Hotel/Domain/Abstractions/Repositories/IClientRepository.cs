using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IClientRepository
    {
        Task AddClient(Client client);
        Task<Client?> GetByNameAndPhoneAsync(FullName name, PhoneNumber phone);
        Task<Client?> GetClientById(Guid id);
        Task<Client?> GetClientByUserId(Guid userId);
        Task<List<Client>> GetClientsByBookingsAsync(List<Booking> bookings);
        Task SaveChangesAsync();
    }
}