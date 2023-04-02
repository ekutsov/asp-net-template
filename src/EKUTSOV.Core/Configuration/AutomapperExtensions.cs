using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EKUTSOV.Core.Configuration;

public static class AutomapperExtensions
{
    public static void AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}

