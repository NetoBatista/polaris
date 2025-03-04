using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Polaris.Domain.Configuration;

namespace Polaris.Test.Configuration;

[TestClass]
public class TokenConfigurationTest
{
    private WebApplicationBuilder _builder;
    public TokenConfigurationTest()
    {
        var secret = Guid.NewGuid().ToString();
        var expire = 5;
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtToken:Secret", secret},
            {"JwtToken:Expire", expire.ToString()},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _builder = WebApplication.CreateBuilder();
        _builder.Configuration.AddConfiguration(configuration);
    }

    [TestMethod("Should be able configure in memory")]
    public void ShoudBeAbleConfigureInMemory()
    {
        TokenConfiguration.Configure(_builder.Configuration);
        Assert.IsNotNull(TokenConfiguration.Secret);
        Assert.IsNotNull(TokenConfiguration.Expire);
    }

    [TestMethod("Should not be able configure secret")]
    public void ShoudNotBeAbleConfigureSecret()
    {
        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        Assert.ThrowsException<Exception>(() => TokenConfiguration.Configure(builder.Configuration));
    }

    [TestMethod("Should not be able configure expire")]
    public void ShoudNotBeAbleConfigureExpire()
    {
        var secret = Guid.NewGuid().ToString();
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtToken:Secret", secret},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        builder.Configuration.AddConfiguration(configuration);
        Assert.ThrowsException<Exception>(() => TokenConfiguration.Configure(builder.Configuration));
    }
}