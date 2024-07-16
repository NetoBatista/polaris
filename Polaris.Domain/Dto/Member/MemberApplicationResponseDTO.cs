namespace Polaris.Domain.Dto.Member
{
    public class MemberApplicationResponseDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public List<MemberItemUserResponseDTO> Users { get; set; } = [];
    }

    public class MemberItemUserResponseDTO
    {
        public Guid MemberId { get; set; }

        public string Auth { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}