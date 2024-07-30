using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class ApplicationUpdateValidator : IValidator<ApplicationUpdateRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private ApplicationUpdateRequestDTO _instance = new();
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationUpdateValidator(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public ValidatorResultModel Validate(ApplicationUpdateRequestDTO request)
        {
            _instance = request;
            NameValidate();
            AlreadyCreatedValidate();
            IdValidate();
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

        private void AlreadyCreatedValidate()
        {
            var entity = ApplicationMapper.ToEntity(_instance);
            var response = _applicationRepository.NameAlreadyExists(entity).Result;
            if (response)
            {
                _resultModel.Errors.Add($"There is already an application with name: {_instance.Name}");
            }
        }

        private void IdValidate()
        {
            var entity = ApplicationMapper.ToEntity(_instance);
            var response = _applicationRepository.Exists(entity).Result;
            if (!response)
            {
                _resultModel.Errors.Add($"No applications found with id: {_instance.Id}");
            }
        }
    }
}
