using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Domain.Validator.Authentication
{
    public class AuthenticationFirebaseValidator : IValidator<AuthenticationFirebaseRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private AuthenticationFirebaseRequestDTO _instance = new();

        public ValidatorResultModel Validate(AuthenticationFirebaseRequestDTO request)
        {
            _instance = request;
            TokenFirebaseValidate();
            FirebaseAppIdValidate();
            FirebaseCredentialsValidate();
            EmailValidate();
            ApplicationValidate();
            return _resultModel;
        }

        private void FirebaseAppIdValidate()
        {
            if (string.IsNullOrEmpty(_instance.FirebaseAppId))
            {
                _resultModel.Errors.Add("JsonCredentials cannot be empty");
            }
        }

        private void FirebaseCredentialsValidate()
        {
            if (string.IsNullOrEmpty(_instance.JsonCredentials))
            {
                _resultModel.Errors.Add("JsonCredentials cannot be empty");
            }
        }

        private void TokenFirebaseValidate()
        {
            if (string.IsNullOrEmpty(_instance.TokenFirebase))
            {
                _resultModel.Errors.Add("TokenFirebase cannot be empty");
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
