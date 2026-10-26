namespace Hotel.API.Contracts
{
    public record BookingRequestExistingClientDto(Guid userId, Guid RoomId, DateTime CheckIn, DateTime CheckOut);
}
