using Microsoft.Extensions.Configuration;

namespace Polaris.Domain.Configuration
{
    public static class ServiceBusConfiguration
    {
        public static string ConnectionString { get; private set; } = string.Empty;
        public static string Queue { get; private set; } = string.Empty;

        public static void Configure(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString") ?? string.Empty;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetSection("ServiceBus")
                                                .GetSection("ConnectionString").Value ?? string.Empty;
            }
            ConnectionString = connectionString;

            var queue = Environment.GetEnvironmentVariable("ServiceBusQueue") ?? string.Empty;
            if (string.IsNullOrEmpty(queue))
            {
                queue = configuration.GetSection("ServiceBus")
                                     .GetSection("Queue").Value ?? string.Empty;
            }
            Queue = queue;
        }
    }
}
