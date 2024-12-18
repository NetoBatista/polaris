using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Authentication
{
    public class AuthenticationRefreshTokenValidator : IValidator<AuthenticationRefreshTokenRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationRefreshTokenRequestDTO _instance = new();

        public ValidatorResultModel Validate(AuthenticationRefreshTokenRequestDTO request)
        {
            _instance = request;
            RefreshTokenValidate();
            return _resultModel;
        }

        private void RefreshTokenValidate()
        {
            if (_instance.RefreshToken == Guid.Empty)
            {
                _resultModel.Errors.Add("RefreshToken invalid!");
            }
        }

    }
}
