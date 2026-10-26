namespace Hotel.API.Contracts
{
    public record   BookingRequestNewClientDto(string PhoneNumber,string FirstName,string LastName,string? Patronymic, 
       Guid RoomId, DateTime CheckIn,DateTime CheckOut);
}
