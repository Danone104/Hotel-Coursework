namespace Hotel.API.Contracts
{
    public record RoomAddRequest(string Title, int Count, string Description,
        decimal Price,IFormFile? Image,string Status);
}
