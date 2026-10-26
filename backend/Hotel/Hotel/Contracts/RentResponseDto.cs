using Hotel.Domain.Enums;

namespace Hotel.API.Contracts
{
    public record RentResponseDto(Guid Id,Guid BookId,string ClientName,string PhoneNum,
        string RoomTitle,DateOnly CheckIn, DateOnly? CheckOut,decimal? price,string Status,string Note);
    
    
}
