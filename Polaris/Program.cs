using Polaris.Configuration;
using Polaris.Domain.Configuration;
using Polaris.Extension;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        DatabaseConfiguration.Configure(builder.Configuration);
        TokenConfiguration.Configure(builder.Configuration);
        ServiceBusConfiguration.Configure(builder.Configuration);

        builder.Services.InjectDependencies();
        builder.Services.ExecuteMigrations();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}