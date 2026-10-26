namespace Hotel.API.Contracts
{
    public record BookingRentHistoryRepsonseDto(Guid Id,string RoomTitle, DateOnly CheckIn,DateOnly CheckOut,string Status,decimal Price);
}
