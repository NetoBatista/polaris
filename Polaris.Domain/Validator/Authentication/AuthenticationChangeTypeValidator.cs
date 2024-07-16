using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Domain.Validator.Application
{
    public class AuthenticationChangeTypeValidator : IValidator<AuthenticationChangeTypeRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationChangeTypeRequestDTO _instance = new();

        public ValidatorResultModel Validate(AuthenticationChangeTypeRequestDTO request)
        {
            _instance = request;
            EmailValidate();
            ApplicationValidate();
            ChangeTypeValidate();
            AuthenticationTypeValidate();
            return _resultModel;
        }

        private void EmailValidate()
        {
            if (string.IsNullOrEmpty(_instance.Email))
            {
                _resultModel.Errors.Add("Email is required");
            }
            else
            {
                var attribute = new EmailAddressAttribute();
                if (!attribute.IsValid(_instance.Email))
                {
                    _resultModel.Errors.Add("Email is not valid");
                }
            }
        }

        private void AuthenticationTypeValidate()
        {
            if (!AuthenticationTypeConstant.IsValid(_instance.Type))
            {
                _resultModel.Errors.Add($"Authentication type must be {AuthenticationTypeConstant.EmailOnly} or {AuthenticationTypeConstant.EmailPassword}");
            }
        }

        private void ApplicationValidate()
        {
            if (_instance.ApplicationId == Guid.Empty)
            {
                _resultModel.Errors.Add("Application invalid!");
            }
        }

        private void ChangeTypeValidate()
        {
            if (_instance.Type == AuthenticationTypeConstant.EmailPassword &&
                string.IsNullOrEmpty(_instance.Password))
            {
                _resultModel.Errors.Add("Password is required");
            }
            else if (_instance.Type == AuthenticationTypeConstant.EmailPassword &&
                     _instance.Password != null &&
                     _instance.Password.Length < 6)
            {
                _resultModel.Errors.Add("Password must have at least 6 characters");
            }
            else if (_instance.Type == AuthenticationTypeConstant.EmailOnly &&
                     !string.IsNullOrEmpty(_instance.Password))
            {
                _resultModel.Errors.Add($"Password must be empty to type {AuthenticationTypeConstant.EmailOnly}");
            }
        }

    }
}
