namespace A2SEVEN.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<RefreshToken> RefreshTokens { get; set; }
}
