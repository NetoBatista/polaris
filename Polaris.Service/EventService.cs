using Azure.Messaging.ServiceBus;
using Polaris.Domain.Configuration;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Model.Event;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Polaris.Service
{
    [ExcludeFromCodeCoverage]
    public class EventService : IEventService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ServiceBusClient? _serviceBusClient;
        public EventService()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            if (string.IsNullOrEmpty(ServiceBusConfiguration.Queue) ||
               string.IsNullOrEmpty(ServiceBusConfiguration.ConnectionString))
            {
                return;
            }

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            _serviceBusClient = new ServiceBusClient(ServiceBusConfiguration.ConnectionString, clientOptions);
        }

        public Task SendMessage(string eventType, object content)
        {
            return SendMessage(eventType, JsonSerializer.Serialize(content, _jsonSerializerOptions));
        }

        public Task SendMessage(string eventType, string content)
        {
            if (_serviceBusClient == null)
            {
                return Task.CompletedTask;
            }

            var data = new EventModel
            {
                Content = content,
                Event = eventType
            };
            var sender = _serviceBusClient.CreateSender(ServiceBusConfiguration.Queue);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(data, _jsonSerializerOptions));
            return sender.SendMessageAsync(message);
        }
    }
}
