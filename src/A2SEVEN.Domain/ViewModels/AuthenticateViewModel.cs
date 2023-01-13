namespace A2SEVEN.Domain.ViewModels;

public class AuthenticateViewModel
{
    public AuthenticateViewModel(string jwtToken, string refreshToken)
    {
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }

    public string JwtToken { get; set; }

    public string RefreshToken { get; set; }
}