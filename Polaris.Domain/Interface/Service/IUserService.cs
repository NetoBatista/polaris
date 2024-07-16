using Polaris.Domain.Dto.User;
using Polaris.Domain.Model;

namespace Polaris.Domain.Interface.Service
{
    public interface IUserService
    {
        Task<ResponseBaseModel> Create(UserCreateRequestDTO request);

        Task<ResponseBaseModel> Update(UserUpdateRequestDTO request);

        Task<ResponseBaseModel> Remove(UserRemoveRequestDTO request);

        Task<ResponseBaseModel> Get(UserGetRequestDTO request);
    }
}
