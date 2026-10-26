namespace Hotel.API.Contracts
{
    public record RegisterRequestDto(string UserName, string Password, string Phone,
            string FirstName, string LastName, string? Patronymic);
}
