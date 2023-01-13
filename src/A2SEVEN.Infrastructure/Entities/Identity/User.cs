using Microsoft.AspNetCore.Identity;

namespace A2SEVEN.Infrastructure.Entities;

public class User : IdentityUser<int>
{
    public bool IsDeleted { get; set; }

    public virtual List<RefreshToken> RefreshTokens { get; set; } = new();
}