using CQRSMediatr;
using EventSourcing.Data;
using EventSourcing.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog;

namespace EventSourcing;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(e =>
        {
            e.AddSerilog();
        });
        services.AddControllers();
        services.AddCqrsMediatr(typeof(Startup));
        services.AddDbContext<EventStoreDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection")));

        // Ensure the required package is installed: Swashbuckle.AspNetCore
        services.AddSwaggerGen(c =>
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var version = assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;
                var assemblyName = assembly.GetName().Name;

                if (version is { })
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = assemblyName,
                        Version = version
                    });

                var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
                if (File.Exists(xmlFile))
                    c.IncludeXmlComments(xmlFile);
            }
        });
    }

    public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            builder.UseDeveloperExceptionPage();
            builder.UseSwagger();
            var assembly = Assembly.GetEntryAssembly();
            builder.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", assembly.FullName.Split(",").First()));
        }

        builder.UseAppExceptionHandler();

        //Allow all CORS
        builder.UseCors("CorsPolicy");

        builder.UseRouting();

        builder.UseAuthentication();
        builder.UseAuthorization();

        builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}