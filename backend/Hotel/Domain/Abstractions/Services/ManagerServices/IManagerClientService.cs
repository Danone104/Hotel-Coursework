using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ManagerServices
{
    public interface IManagerClientService
    {
        Task<Client> GetClientByIdAsync(Guid id);
        Task<List<Client>> GetClientsForBookingsAsync(List<Booking> bookings);
    }
}
