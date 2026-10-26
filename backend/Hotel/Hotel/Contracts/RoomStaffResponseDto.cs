namespace Hotel.API.Contracts
{
    public record RoomStaffResponseDto(Guid id,string Title, int Count, string Description, decimal Price,string Status);
}
