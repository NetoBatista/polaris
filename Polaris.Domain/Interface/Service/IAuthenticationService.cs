using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Model;

namespace Polaris.Domain.Interface.Service
{
    public interface IAuthenticationService
    {
        Task<ResponseBaseModel> Authenticate(AuthenticationRequestDTO request);

        Task<ResponseBaseModel> GenerateCode(AuthenticationGenerateCodeRequestDTO request);

        Task<ResponseBaseModel> RefreshToken(AuthenticationRefreshTokenRequestDTO request);

        Task<ResponseBaseModel> ChangeType(AuthenticationChangeTypeRequestDTO request);

        Task<ResponseBaseModel> ChangePassword(AuthenticationChangePasswordRequestDTO request);
    }
}
