namespace Polaris.Domain.Model.Event
{
    public class EventAuthenticationModel
    {
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}
