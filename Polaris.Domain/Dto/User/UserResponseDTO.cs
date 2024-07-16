namespace Polaris.Domain.Dto.User
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string Email { get; set; } = null!;

        public string Language { get; set; } = null!;
    }
}
