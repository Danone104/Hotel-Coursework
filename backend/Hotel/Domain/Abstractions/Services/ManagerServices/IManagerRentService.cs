using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ManagerServices
{
    public interface IManagerRentService
    {
        Task AddNoteAsync(Guid rentId, string note);
        Task FixCheckout(Guid rentId);
        Task<List<RentRoom>> GetActiveRentsAsync();
        Task RentRoom(Guid bookId);
    }
}

