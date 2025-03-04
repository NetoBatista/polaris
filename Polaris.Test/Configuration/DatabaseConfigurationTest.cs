using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Polaris.Domain.Configuration;

namespace Polaris.Test.Configuration;

[TestClass]
public class DatabaseConfigurationTest
{
    private WebApplicationBuilder _builder;
    public DatabaseConfigurationTest()
    {
        var connectionString = Guid.NewGuid().ToString();
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Database:ConnectionString", connectionString}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _builder = WebApplication.CreateBuilder();
        _builder.Configuration.AddConfiguration(configuration);
    }

    [TestCleanup]
    public void Teardown()
    {
        Environment.SetEnvironmentVariable("ConnectionString", null);
    }

    [TestMethod("Should be able configure in memory")]
    public void ShoudBeAbleConfigureInMemory()
    {
        DatabaseConfiguration.Configure(_builder.Configuration);
        Assert.IsNotNull(DatabaseConfiguration.ConnectionString);
    }

    [TestMethod("Should be able configure environment")]
    public void ShoudBeAbleConfigureEnvironment()
    {
        var connectionString = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("ConnectionString", connectionString);

        DatabaseConfiguration.Configure(_builder.Configuration);
        Assert.AreEqual(DatabaseConfiguration.ConnectionString, connectionString);
    }

    [TestMethod("Should not be able configure")]
    public void ShoudNotBeAbleConfigure()
    {
        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        Assert.ThrowsException<Exception>(() => DatabaseConfiguration.Configure(builder.Configuration));
    }
}