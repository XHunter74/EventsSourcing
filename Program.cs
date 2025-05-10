using Serilog;
using EventSourcing.Extensions;

namespace EventSourcing;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .ApplyDbMigrations()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)); 
    }
}
