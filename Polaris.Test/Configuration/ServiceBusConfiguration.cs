using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Polaris.Domain.Configuration;

namespace Polaris.Test.Configuration;

[TestClass]
public class ServiceBusConfigurationTest
{
    private WebApplicationBuilder _builder;
    public ServiceBusConfigurationTest()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ServiceBus:ConnectionString", Guid.NewGuid().ToString()},
            {"ServiceBus:Queue", Guid.NewGuid().ToString()},
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
        ServiceBusConfiguration.Configure(_builder.Configuration);
        Assert.IsNotNull(ServiceBusConfiguration.ConnectionString);
        Assert.IsNotNull(ServiceBusConfiguration.Queue);
    }

    [TestMethod("Should not be able configure")]
    public void ShoudNotBeAbleConfigure()
    {
        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        ServiceBusConfiguration.Configure(builder.Configuration);
        Assert.AreEqual(ServiceBusConfiguration.ConnectionString, string.Empty);
        Assert.AreEqual(ServiceBusConfiguration.Queue, string.Empty);
    }

    [TestMethod("Should not be able configure queue")]
    public void ShoudNotBeAbleConfigureQueue()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ServiceBus:ConnectionString", Guid.NewGuid().ToString()},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        builder.Configuration.AddConfiguration(configuration);

        ServiceBusConfiguration.Configure(builder.Configuration);
        Assert.AreNotEqual(ServiceBusConfiguration.ConnectionString, string.Empty);
        Assert.AreEqual(ServiceBusConfiguration.Queue, string.Empty);
    }

    [TestMethod("Should not be able configure ConnectionString")]
    public void ShoudNotBeAbleConfigureConnectionString()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ServiceBus:Queue", Guid.NewGuid().ToString()},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var options = new WebApplicationOptions();
        var builder = WebApplication.CreateEmptyBuilder(options);
        builder.Configuration.AddConfiguration(configuration);

        ServiceBusConfiguration.Configure(builder.Configuration);
        Assert.AreEqual(ServiceBusConfiguration.ConnectionString, string.Empty);
        Assert.AreNotEqual(ServiceBusConfiguration.Queue, string.Empty);
    }
}