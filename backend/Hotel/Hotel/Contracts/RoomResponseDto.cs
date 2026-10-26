namespace Hotel.API.Contracts
{
    public record RoomResponseDto(Guid RoomId, string Title, string Description, int CountPerson, decimal Price);
}
