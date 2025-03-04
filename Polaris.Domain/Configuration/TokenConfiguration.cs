using Microsoft.Extensions.Configuration;

namespace Polaris.Domain.Configuration
{
    public static class TokenConfiguration
    {
        public static string Secret { get; private set; } = string.Empty;
        public static int Expire { get; private set; } = 0;

        public static void Configure(IConfiguration configuration)
        {
            var secretToken = Environment.GetEnvironmentVariable("JwtTokenSecret") ?? string.Empty;

            if (string.IsNullOrEmpty(secretToken))
            {
                secretToken = configuration.GetSection("JwtToken")
                                           .GetSection("Secret").Value ?? string.Empty;
            }
            if (string.IsNullOrEmpty(secretToken))
            {
                throw new Exception("JWT Token Secret not found");
            }

            var expire = Environment.GetEnvironmentVariable("JwtTokenExpire") ?? string.Empty;
            if (string.IsNullOrEmpty(expire))
            {
                expire = configuration.GetSection("JwtToken")
                                      .GetSection("Expire").Value ?? string.Empty;
            }
            if (string.IsNullOrEmpty(expire))
            {
                throw new Exception("JWT Token Expire not found");
            }

            Secret = secretToken;
            Expire = int.Parse(expire);
        }
    }
}
