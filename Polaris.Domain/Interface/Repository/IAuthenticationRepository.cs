using Polaris.Domain.Entity;
using Polaris.Domain.Model.Authentication;

namespace Polaris.Domain.Interface.Repository
{
    public interface IAuthenticationRepository
    {
        Task<Authentication> Create(Authentication authentication);

        Task<Authentication?> GetById(Guid id);

        Task<bool> AuthenticatePassword(AuthenticationPasswordModel model);

        Task<bool> ChangePassword(Authentication authentication);

        Task<Authentication> GenerateCode(Authentication authentication);

        Task<bool> CanValidateCode(Authentication authentication);

        Task<bool> AuthenticateCode(Authentication authentication);

        Task<Authentication?> GetByEmailApplication(AuthenticationByUserApplicationModel model);

        Task ClearCodeConfirmation(Authentication authentication);
    }
}