using System.Text.Json.Serialization;

namespace Polaris.Domain.Dto.Application
{
    public class ApplicationUpdateRequestDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
