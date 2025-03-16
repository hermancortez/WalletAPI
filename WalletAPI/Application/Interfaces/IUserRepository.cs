using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        void AddLoginHistory(LoginHistory loginHistory);
        Task<User?> GetUserByUsername(string username);
        Task AddUser(User user);
    }
}
