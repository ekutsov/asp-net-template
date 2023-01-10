namespace A2SEVEN.Infrastructure.Context;

#if (Authorization == JWT)

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}

#elif (Authorization == NoAuth)

public class AppDbContext : DbContext {
	 public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

#endif