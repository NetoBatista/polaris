
namespace Polaris.Domain.Dto.Member
{
    public class MemberCreateRequestDTO
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public string? Password { get; set; }
    }
}