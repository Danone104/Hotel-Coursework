namespace Hotel.API.Contracts
{
    public record RentHistoryResponseDto(Guid Id, string RoomTitle, DateOnly CheckIn, DateOnly CheckOut, decimal Price);
}
