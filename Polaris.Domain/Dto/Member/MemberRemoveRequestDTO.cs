using System.Text.Json.Serialization;
namespace Polaris.Domain.Dto.Member
{
    public class MemberRemoveRequestDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}