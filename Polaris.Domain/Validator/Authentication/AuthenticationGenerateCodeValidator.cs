using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Domain.Validator.Authentication
{
    public class AuthenticationGenerateCodeValidator : IValidator<AuthenticationGenerateCodeRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationGenerateCodeRequestDTO _instance = new();

        public ValidatorResultModel Validate(AuthenticationGenerateCodeRequestDTO request)
        {
            _instance = request;
            EmailValidate();
            ApplicationValidate();
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

        private void ApplicationValidate()
        {
            if (_instance.ApplicationId == Guid.Empty)
            {
                _resultModel.Errors.Add("Application invalid!");
            }
        }

    }
}
