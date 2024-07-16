using Polaris.Domain.Constant;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Domain.Validator.Application
{
    public class UserCreateValidator : IValidator<UserCreateRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private UserCreateRequestDTO _instance = new();

        public ValidatorResultModel Validate(UserCreateRequestDTO request)
        {
            _instance = request;
            NameValidate();
            EmailValidate();
            LanguageValidate();
            return _resultModel;
        }

        private void NameValidate()
        {
            if (string.IsNullOrEmpty(_instance.Name))
            {
                _resultModel.Errors.Add("Name is required");
            }
            else if (_instance.Name.Length < 3)
            {
                _resultModel.Errors.Add("Name must have at least 3 characters");
            }
            else if (_instance.Name.Length > 256)
            {
                _resultModel.Errors.Add("Name cannot be longer than 256 characters");
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

        private void LanguageValidate()
        {
            if (string.IsNullOrEmpty(_instance.Language))
            {
                _resultModel.Errors.Add("Language is required");
            }
            else if (!UserLanguageConstant.IsValid(_instance.Language))
            {
                _resultModel.Errors.Add($"Auth must be {UserLanguageConstant.PT_BR} or {UserLanguageConstant.EN_US}");
            }
        }
    }
}
