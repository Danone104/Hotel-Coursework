using Hotel.Domain.ValueObjects;

namespace Hotel.API.Contracts
{
    public record EmployeeResponseDto( Guid Id,string UserName,string Name, string TelephoneNum);
}
