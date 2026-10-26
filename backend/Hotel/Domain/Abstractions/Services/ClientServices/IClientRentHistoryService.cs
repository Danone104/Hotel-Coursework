using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ClientServices
{
    public interface IClientRentHistoryService
    {
        Task CancelActiveBooking(Guid userId, Guid bookId);
        Task<List<Booking>> GetActiveBookingsAsync(Guid userId, int page, int pageSize);
        Task<List<Booking>> GetCancelledBookingsAsync(Guid userId, int page, int pageSize);
        Task<List<RentRoom>> GetRentsHistoryAsync(Guid userId, int page, int pageSize);
        Task<List<Booking>> GetRentsBookingsAsync(List<RentRoom> rents);
    }
}
