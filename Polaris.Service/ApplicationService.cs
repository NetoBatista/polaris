using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IValidator<ApplicationCreateRequestDTO> _createValidator;
        private readonly IValidator<ApplicationUpdateRequestDTO> _updateValidator;
        private readonly IValidator<ApplicationRemoveRequestDTO> _removeValidator;
        public ApplicationService(IApplicationRepository applicationRepository,
                                  IValidator<ApplicationCreateRequestDTO> createValidator,
                                  IValidator<ApplicationUpdateRequestDTO> updateValidator,
                                  IValidator<ApplicationRemoveRequestDTO> removeValidator)
        {
            _applicationRepository = applicationRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _removeValidator = removeValidator;
        }

        public async Task<ResponseBaseModel> Create(ApplicationCreateRequestDTO request)
        {
            var responseValidate = _createValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = ApplicationMapper.ToEntity(request);
            var response = await _applicationRepository.Create(entity);
            return ResponseBaseModel.Ok(ApplicationMapper.ToResponseDTO(response));
        }

        public async Task<ResponseBaseModel> Update(ApplicationUpdateRequestDTO request)
        {
            var responseValidate = _updateValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = ApplicationMapper.ToEntity(request);
            var response = await _applicationRepository.Update(entity);
            return ResponseBaseModel.Ok(ApplicationMapper.ToResponseDTO(response));
        }

        public async Task<ResponseBaseModel> Remove(ApplicationRemoveRequestDTO request)
        {
            var responseValidate = _removeValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }
            var entity = ApplicationMapper.ToEntity(request);
            await _applicationRepository.Remove(entity);
            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> GetAll()
        {
            var response = await _applicationRepository.GetAll();
            return ResponseBaseModel.Ok(ApplicationMapper.ToResponseDTO(response));
        }
    }
}
