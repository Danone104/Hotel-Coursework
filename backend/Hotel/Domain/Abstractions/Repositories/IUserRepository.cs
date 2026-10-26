using Hotel.Domain.Models;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<bool> IsSystemHaveAdminsAsync();
        Task AddUser(User user);
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByUserName(string userName);
        Task<List<User>> GetUsersEmployeesAsync();
        Task DeleteUserAsync(User user);
        Task SaveChangesAsync();
    }
}