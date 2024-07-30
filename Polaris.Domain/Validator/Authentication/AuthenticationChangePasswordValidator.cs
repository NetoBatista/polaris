using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
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
            PasswordTypeValidate(entity);
            PasswordValidate(entity);
            return _resultModel;
        }

        private void ApplicationValidate(Authentication? authentication)
        {
            if (authentication == null)
            {
                _resultModel.Errors.Add("User not found");
            }
        }

        private void PasswordTypeValidate(Authentication? authentication)
        {
            if (authentication == null)
            {
                return;
            }
            if (authentication.Type != AuthenticationTypeConstant.EmailPassword)
            {
                _resultModel.Errors.Add($"Password can only be changed with the type {AuthenticationTypeConstant.EmailPassword}");
            }
        }

        private void PasswordValidate(Authentication? authentication)
        {
            if (authentication == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_instance.CurrentPassword))
            {
                _resultModel.Errors.Add("CurrentPassword is required");
            }
            else if (_instance.CurrentPassword.Length < 6)
            {
                _resultModel.Errors.Add($"CurrentPassword must have at least 6 characters");
            }
            else if (authentication.Password != CryptographyUtil.ConvertToMD5(_instance.CurrentPassword))
            {
                _resultModel.Errors.Add("Passwords don't match");
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
