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
        services.AddTransient<UserManager<User>>();
        
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "A2SEVEN.API.xml");
            c.IncludeXmlComments(filePath);
        });
    }
}