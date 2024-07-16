using Polaris.Domain.Dto.Member;
using Polaris.Domain.Entity;

namespace Polaris.Domain.Mapper
{
    public static class MemberMapper
    {
        public static Member ToEntity(MemberCreateRequestDTO request)
        {
            return new Member
            {
                ApplicationId = request.ApplicationId,
                UserId = request.UserId,
            };
        }

        public static Member ToEntity(MemberRemoveRequestDTO request)
        {
            return new Member
            {
                Id = request.Id
            };
        }

    }
}
