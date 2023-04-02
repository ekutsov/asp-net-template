namespace EKUTSOV.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticateViewModel> AuthenticateAsync(AuthenticateDTO loginDto);

    Task<AuthenticateViewModel> RefreshTokenAsync(string token);

    Task RevokeTokenAsync(string token);

    Task ForgotPassword(ForgotPasswordDTO forgotPasswordDto);

    Task<ResetPasswordViewModel> ResetPassword(ResetPasswordDTO resetPasswordInfo);
}