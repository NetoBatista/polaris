using Polaris.Domain.Configuration;

namespace Polaris.Configuration
{
    public static class TokenConfiguration
    {
        public static void ConfigureToken(this WebApplicationBuilder app)
        {
            var secretToken = app.Configuration.GetSection("JwtToken")
                                               .GetValue<string>("Secret") ?? string.Empty;
            if (string.IsNullOrEmpty(secretToken))
            {
                throw new Exception("JWT Token Secret not found");
            }

            var expire = app.Configuration.GetSection("JwtToken")
                                          .GetValue<int?>("Expire") ?? 0;
            if (expire == 0)
            {
                throw new Exception("JWT Token Expire not found");
            }

            TokenConfig.Secret = secretToken;
            TokenConfig.Expire = expire;
        }
    }
}
