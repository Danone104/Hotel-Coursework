using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IBookingRepository
    {
        Task AddBook(Booking book);
        Task<List<Booking>> GetActiveAndCancelledForDateAsync(DateOnly date);
        Task<List<Booking>> GetActiveBookingAsync();
        Task<List<Booking>> GetActiveBookingForDateAsync(DateOnly date);
        Task<List<Booking>> GetActiveBookingsByRoomAsync(Guid roomId);
        Task<List<Booking>> GetActiveByClientByPageAsync(Guid clientId, int page, int pageSize);
        Task<List<Booking>> GetActiveRentedBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(Guid id);
        Task<List<Booking>> GetCancelledByClientByPageAsync(Guid clientId, int page, int pageSize);
        Task<List<Booking>> GetBookingsForRentsAsync(List<RentRoom> rents);
        Task<List<Booking>> GetForRentHistorytByPageAsync(Guid clientId, int page, int pageSize);
        Task<List<Booking>> GetOverlappingWithPeriodAsync(Guid roomId, DateOnly startDate, DateOnly endDate);
        Task<List<Booking>> GetRentedBookingsAsync();
        Task<List<Booking>> GetTodaysActiveAndCancelledAsync();
        Task<List<Booking>> GetTodaysActiveBookingAsync();
        Task SaveChangesAsync();


    }
}
