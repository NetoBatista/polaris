using Polaris.Domain.Configuration;

namespace Polaris.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureDatabase(this WebApplicationBuilder app)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? string.Empty;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = app.Configuration.GetSection("Database")
                                                    .GetValue<string>("ConnectionString") ?? string.Empty;
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString not found");
            }
            DatabaseConfig.ConnectionString = connectionString;
        }
    }
}
