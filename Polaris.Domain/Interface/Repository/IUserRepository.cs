using Polaris.Domain.Entity;

namespace Polaris.Domain.Interface.Repository
{
    public interface IUserRepository
    {
        Task<User> Create(User user);

        Task<bool> AlreadyCreated(User user);

        Task<User> Update(User user);

        Task Remove(User user);

        Task<User?> Get(User user);

        Task<bool> Exists(User user);
    }
}
