using EventSourcing.Extensions;
using EventSourcing.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EventSourcing;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.AddRabbitMQClient(connectionName: "Rabbit");

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        startup.Configure(app, env);

        app.ApplyDbMigrations();

        app.MapDefaultEndpoints();

        app.Run();
    }
}
