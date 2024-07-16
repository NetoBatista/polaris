using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Domain.Validator.Application
{
    public class AuthenticationValidator : IValidator<AuthenticationRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationRequestDTO _instance = new();

        public ValidatorResultModel Validate(AuthenticationRequestDTO request)
        {
            _instance = request;
            CodePasswordValidate();
            EmailValidate();
            ApplicationValidate();
            return _resultModel;
        }

        private void CodePasswordValidate()
        {
            if (string.IsNullOrEmpty(_instance.Password) && string.IsNullOrEmpty(_instance.Code))
            {
                _resultModel.Errors.Add("Code or Password cannot be empty");
            }
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

        private void ApplicationValidate()
        {
            if (_instance.ApplicationId == Guid.Empty)
            {
                _resultModel.Errors.Add("Application invalid!");
            }
        }

    }
}
