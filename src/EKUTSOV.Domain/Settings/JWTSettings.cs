namespace EKUTSOV.Domain.Settings;

public class JWTSettings
{
    public string Secret { get; set; }

    public string Audience { get; set; }

    public string Issuer { get; set; }

    public int AccessTokenExpireSeconds { get; set; }

    public int RefreshTokenExpireSeconds { get; set; }
}