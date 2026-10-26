namespace Hotel.API.Contracts
{
    public record RoomUpdateRequestDto(string Title, int Count, string Description, decimal Price, IFormFile? Image);
}
