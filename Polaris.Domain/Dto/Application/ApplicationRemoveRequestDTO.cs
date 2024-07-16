using System.Text.Json.Serialization;

namespace Polaris.Domain.Dto.Application
{
    public class ApplicationRemoveRequestDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}
