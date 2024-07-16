using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class ApplicationCreateValidator : IValidator<ApplicationCreateRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private ApplicationCreateRequestDTO _instance = new();
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationCreateValidator(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public ValidatorResultModel Validate(ApplicationCreateRequestDTO request)
        {
            _instance = request;
            NameValidate();
            AlreadyCreatedValidate();
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
        }

        private void AlreadyCreatedValidate()
        {
            var entity = ApplicationMapper.ToEntity(_instance);
            var response = _applicationRepository.AlreadyCreated(entity).Result;
            if (response)
            {
                _resultModel.Errors.Add($"There is already an application with that name: {_instance.Name}");
            }
        }
    }
}
