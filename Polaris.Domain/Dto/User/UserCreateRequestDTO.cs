namespace Polaris.Domain.Dto.User
{
    public class UserCreateRequestDTO
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Language { get; set; } = null!;
    }
}
