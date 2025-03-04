namespace Polaris.Domain.Model.Event
{
    public class EventModel
    {
        public string Event { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public readonly DateTime PublishedAt = DateTime.UtcNow;
    }
}
