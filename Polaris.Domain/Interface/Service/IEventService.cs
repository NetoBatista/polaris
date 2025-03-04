namespace Polaris.Domain.Interface.Service
{
    public interface IEventService
    {
        Task SendMessage(string eventType, object content);

        Task SendMessage(string eventType, string content);
    }
}
