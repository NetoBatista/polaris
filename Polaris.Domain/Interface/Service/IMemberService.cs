using Polaris.Domain.Dto.Member;
using Polaris.Domain.Model;

namespace Polaris.Domain.Interface.Service
{
    public interface IMemberService

    {
        Task<ResponseBaseModel> Create(MemberCreateRequestDTO request);

        Task<ResponseBaseModel> Remove(MemberRemoveRequestDTO request);

        Task<ResponseBaseModel> GetByUser(MemberGetUserRequestDTO request);

        Task<ResponseBaseModel> GetByApplication(MemberGetApplicationRequestDTO request);
    }
}
