using Polaris.Domain.Constant;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class UserUpdateValidator : IValidator<UserUpdateRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();

        private UserUpdateRequestDTO _instance = new();

        private IUserRepository _userRepository;

        public UserUpdateValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ValidatorResultModel Validate(UserUpdateRequestDTO request)
        {
            _instance = request;
            NameValidate();
            LanguageValidate();
            UserNotExistsValidate();
            return _resultModel;
        }

        private void NameValidate()
        {
            if (string.IsNullOrEmpty(_instance.Name))
            {
                return;
            }

            if (_instance.Name.Length < 3)
            {
                _resultModel.Errors.Add("Name must have at least 3 characters");
            }
            else if (_instance.Name.Length > 256)
            {
                _resultModel.Errors.Add("Name cannot be longer than 256 characters");
            }
        }

        private void UserNotExistsValidate()
        {
            var alreadyCreated = _userRepository.Exists(UserMapper.ToEntity(_instance)).Result;
            if (!alreadyCreated)
            {
                _resultModel.Errors.Add("User not found");
            }
        }

        private void LanguageValidate()
        {
            if (string.IsNullOrEmpty(_instance.Language))
            {
                return;
            }

            if (!UserLanguageConstant.IsValid(_instance.Language))
            {
                _resultModel.Errors.Add($"Auth must be {UserLanguageConstant.PT_BR} or {UserLanguageConstant.EN_US}");
            }
        }
    }
}
