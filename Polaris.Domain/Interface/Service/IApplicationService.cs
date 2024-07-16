using Polaris.Domain.Dto.Application;
using Polaris.Domain.Model;

namespace Polaris.Domain.Interface.Service
{
    public interface IApplicationService
    {
        Task<ResponseBaseModel> Create(ApplicationCreateRequestDTO request);

        Task<ResponseBaseModel> Update(ApplicationUpdateRequestDTO request);

        Task<ResponseBaseModel> Remove(ApplicationRemoveRequestDTO request);

        Task<ResponseBaseModel> GetAll();
    }
}
