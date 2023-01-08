namespace A2SEVEN.Domain.DTO;

public class ResetPasswordDTO
{
    public string Email { get; set; }

    public string Token { get; set; }

    public string Password { get; set; }
}