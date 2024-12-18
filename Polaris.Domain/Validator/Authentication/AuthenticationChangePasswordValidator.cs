using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Authentication
{
    public class AuthenticationChangePasswordValidator : IValidator<AuthenticationChangePasswordRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationChangePasswordRequestDTO _instance = new();
        private IAuthenticationRepository _authenticationRepository;
        public AuthenticationChangePasswordValidator(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public ValidatorResultModel Validate(AuthenticationChangePasswordRequestDTO request)
        {
            _instance = request;
            var model = new AuthenticationByUserApplicationModel
            {
                ApplicationId = request.ApplicationId,
                Email = request.Email
            };
            var entity = _authenticationRepository.GetByEmailApplication(model).Result;
            ApplicationValidate(entity);
            PasswordValidate(entity);
            return _resultModel;
        }

        private void ApplicationValidate(Entity.Authentication? authentication)
        {
            if (authentication == null)
            {
                _resultModel.Errors.Add("User not found");
            }
        }

        private void PasswordValidate(Entity.Authentication? authentication)
        {
            if (authentication == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_instance.Password))
            {
                _resultModel.Errors.Add("Password is required");
            }
            else if (_instance.Password.Length < 6)
            {
                _resultModel.Errors.Add($"Password must have at least 6 characters");
            }

        }
    }
}
