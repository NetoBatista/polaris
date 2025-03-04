using Microsoft.Extensions.Configuration;
namespace Polaris.Domain.Configuration
{
    public static class DatabaseConfiguration
    {
        public static string ConnectionString { get; private set; } = string.Empty;

        public static void Configure(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? string.Empty;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetSection("Database")
                                                .GetSection("ConnectionString").Value ?? string.Empty;
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString not found");
            }
            ConnectionString = connectionString;
        }
    }
}
