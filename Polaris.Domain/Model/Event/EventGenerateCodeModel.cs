namespace Polaris.Domain.Model.Event
{
    public class EventGenerateCodeModel
    {
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
