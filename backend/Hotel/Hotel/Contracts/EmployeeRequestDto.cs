using Hotel.Domain.Enums;

namespace Hotel.API.Contracts
{
    public record EmployeeRequestDto(string UserName,string Password, string FirstName, string LastName,
          string? Patronymic, string Phone, string Role);
}
