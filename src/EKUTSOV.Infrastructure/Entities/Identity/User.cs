using Microsoft.AspNetCore.Identity;

namespace EKUTSOV.Infrastructure.Entities;

public class User : IdentityUser<int>
{
    public bool IsDeleted { get; set; }

    public virtual List<RefreshToken> RefreshTokens { get; set; } = new();
}