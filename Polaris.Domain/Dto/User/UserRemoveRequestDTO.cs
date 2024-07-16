using System.Text.Json.Serialization;

namespace Polaris.Domain.Dto.User
{
    public class UserRemoveRequestDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}
