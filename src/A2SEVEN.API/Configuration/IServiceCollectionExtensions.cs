using System.IdentityModel.Tokens.Jwt;
using System.Text;
using A2SEVEN.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

    public static void SetupAuthorization(this IServiceCollection services)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetService<IOptions<JWTSettings>>();
        services.AddIdentityAndConfigure();
        ConfigureJwt(services, jwtOptions?.Value);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder
                .WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });
    }
    private static void ConfigureJwt(IServiceCollection services, JWTSettings? jwtSettings)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = false;
            cfg.TokenValidationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings?.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtSettings?.Secret ?? string.Empty)),
                RoleClaimType = CustomClaims.Roles,
                ClockSkew = TimeSpan.Zero
            };
        });
    }
    public static void AddIdentityAndConfigure(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = false;
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            // User settings
            options.User.RequireUniqueEmail = true;
        });
        services.AddIdentity<User, Role>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
}