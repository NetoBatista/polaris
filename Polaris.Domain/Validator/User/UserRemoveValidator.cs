using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class UserRemoveValidator : IValidator<UserRemoveRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();

        private UserRemoveRequestDTO _instance = new();

        private IUserRepository _userRepository;

        public UserRemoveValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ValidatorResultModel Validate(UserRemoveRequestDTO request)
        {
            _instance = request;
            UserNotExistsValidate();
            return _resultModel;
        }
        private void UserNotExistsValidate()
        {
            var alreadyCreated = _userRepository.Exists(UserMapper.ToEntity(_instance)).Result;
            if (!alreadyCreated)
            {
                _resultModel.Errors.Add("User not found");
            }
        }
    }
}
