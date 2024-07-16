using System.Text.Json.Serialization;

namespace Polaris.Domain.Dto.User
{
    public class UserUpdateRequestDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;
    }
}
