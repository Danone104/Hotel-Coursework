
namespace Hotel.API.Contracts
{
    public record BookingResponseDto(Guid Id,Guid ClientId,Guid RoomId,string RoomTitle,string ClientName,string PhoneNum,
        DateOnly CheckIn, DateOnly CheckOut,string Status,decimal Price,string? ReasonCancel);
}
