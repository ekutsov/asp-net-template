using A2SEVEN.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace A2SEVEN.API.Configuration;

public static class IServiceCollectionExtensions
{
    public static void AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DatabaseConnection"), 
                npgSqlOptions => npgSqlOptions.EnableRetryOnFailure()
            );
        });
    }

    public static void AddServices(this IServiceCollection services)
    {
        // TODO: Add application services
    }
}