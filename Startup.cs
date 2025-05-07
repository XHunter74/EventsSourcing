using CQRSMediatr;
using EventSourcing.Data;
using Microsoft.EntityFrameworkCore;

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
        services.AddControllers();
        services.AddCqrsMediatr(typeof(Startup));
        services.AddDbContext<EventStoreDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection")));
    }

    public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            builder.UseDeveloperExceptionPage();
        }

        //Allow all CORS
        builder.UseCors("CorsPolicy");

        builder.UseRouting();

        builder.UseAuthentication();
        builder.UseAuthorization();

        builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}