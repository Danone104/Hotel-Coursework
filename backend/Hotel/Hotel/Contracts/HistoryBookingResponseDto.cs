namespace Hotel.API.Contracts
{
    public record HistoryBookingResponseDto(Guid Id,string RoomTitle, DateOnly CheckIn, DateOnly CheckOut,string ReasonCancel);
}
