namespace A2SEVEN.Infrastructure.Context;

#if (Authorization == JWTAuth)
public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
#else
public class AppDbContext : DbContext {
	 public AppDbContext(DbContextOptions options) : base(options) { }
}
#endif