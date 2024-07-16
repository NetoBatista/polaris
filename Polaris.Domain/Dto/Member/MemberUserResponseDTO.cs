namespace Polaris.Domain.Dto.Member
{
    public class MemberUserResponseDTO
    {
        public string Email { get; set; } = null!;

        public List<MemberItemApplicationResponseDTO> Applications { get; set; } = [];
    }

    public class MemberItemApplicationResponseDTO
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public string Name { get; set; } = null!;

        public string Auth { get; set; } = null!;
    }
}