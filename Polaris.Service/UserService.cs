using Polaris.Domain.Dto.User;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserCreateRequestDTO> _createValidator;
        private readonly IValidator<UserUpdateRequestDTO> _updateValidator;
        private readonly IValidator<UserRemoveRequestDTO> _removeValidator;
        public UserService(IUserRepository userRepository,
                           IValidator<UserCreateRequestDTO> createValidator,
                           IValidator<UserUpdateRequestDTO> updateValidator,
                           IValidator<UserRemoveRequestDTO> removeValidator)
        {
            _userRepository = userRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _removeValidator = removeValidator;
        }
        public async Task<ResponseBaseModel> Create(UserCreateRequestDTO request)
        {
            var responseValidate = _createValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = UserMapper.ToEntity(request);
            var response = await _userRepository.Create(entity);
            return ResponseBaseModel.Ok(UserMapper.ToResponseDTO(response));
        }

        public async Task<ResponseBaseModel> Get(UserGetRequestDTO request)
        {
            List<UserResponseDTO> response = [];
            if (!string.IsNullOrEmpty(request.Email))
            {
                var entity = new User { Email = request.Email };
                var users = await _userRepository.Get(entity);
                if (users != null)
                {
                    response.Add(UserMapper.ToResponseDTO(users));
                }
            }
            else
            {
                var users = await _userRepository.Get();
                response.AddRange(users.Select(UserMapper.ToResponseDTO));
            }

            return ResponseBaseModel.Ok(response);
        }

        public async Task<ResponseBaseModel> Remove(UserRemoveRequestDTO request)
        {
            var responseValidate = _removeValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = UserMapper.ToEntity(request);
            await _userRepository.Remove(entity);
            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> Update(UserUpdateRequestDTO request)
        {
            var responseValidate = _updateValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = UserMapper.ToEntity(request);
            await _userRepository.Update(entity);
            return ResponseBaseModel.Ok();
        }
    }
}
