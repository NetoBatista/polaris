using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class ApplicationRemoveValidator : IValidator<ApplicationRemoveRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private ApplicationRemoveRequestDTO _instance = new();
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationRemoveValidator(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public ValidatorResultModel Validate(ApplicationRemoveRequestDTO request)
        {
            _instance = request;
            AnyMemberValidate();
            ExistsApplicationValidate();
            return _resultModel;
        }

        private void AnyMemberValidate()
        {
            var entity = ApplicationMapper.ToEntity(_instance);
            var response = _applicationRepository.AnyMember(entity).Result;
            if (response)
            {
                _resultModel.Errors.Add("It is not possible to remove, there are still members associated with this application.");
            }
        }

        private void ExistsApplicationValidate()
        {
            var entity = ApplicationMapper.ToEntity(_instance);
            var exists = _applicationRepository.Exists(entity).Result;
            if (!exists)
            {
                _resultModel.Errors.Add($"Application not found");
            }
        }
    }
}
