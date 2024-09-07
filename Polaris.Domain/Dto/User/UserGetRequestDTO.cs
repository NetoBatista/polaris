namespace Polaris.Domain.Dto.User
{
    public class UserGetRequestDTO
    {
        public Guid? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public Guid? ApplicationId { get; set; }
    }
}
