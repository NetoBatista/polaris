using Polaris.Domain.Entity;

namespace Polaris.Domain.Interface.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> Create(Guid authenticationId);
        Task<RefreshToken?> Get(RefreshToken entity);
        Task Remove(RefreshToken entity);
    }
}
