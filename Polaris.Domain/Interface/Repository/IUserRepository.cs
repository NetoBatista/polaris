using Polaris.Domain.Entity;

namespace Polaris.Domain.Interface.Repository
{
    public interface IUserRepository
    {
        Task<User> Create(User user);

        Task<bool> Exists(User user);

        Task<bool> Update(User user);

        Task<bool> Remove(User user);

        Task<User?> Get(User user);

        Task<List<User>> Get();
    }
}
