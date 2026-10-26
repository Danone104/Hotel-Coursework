namespace Hotel.API.Contracts
{
    public record UserProfileResponse(Guid Id, string Role, string login);
}
