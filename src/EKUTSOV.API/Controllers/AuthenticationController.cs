namespace EKUTSOV.API.Controllers;

[AllowAnonymous]
[Route("api")]
public class AuthenticationController : BaseController<IAuthenticationService>
{
    public AuthenticationController(IAuthenticationService service) : base(service) { }

    /// <summary>
    /// Login in to the system using the specified credentials
    /// </summary>
    /// <param name="authenticateDto">username and password</param>
    /// <returns>Access token</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticateViewModel>> Authenticate([FromBody] AuthenticateDTO authenticateDto)
    {
        AuthenticateViewModel response = await _service.AuthenticateAsync(authenticateDto);

        return response;
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <returns>Success result</returns>
    [HttpPost("token/refresh")]
    public async Task<ActionResult<AuthenticateViewModel>> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
    {
        AuthenticateViewModel response = await _service.RefreshTokenAsync(refreshTokenDto.Token);

        return response;
    }

    /// <summary>
    /// Revoke token
    /// </summary>
    /// <returns>Success result</returns>
    [HttpPost("token/revoke")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task RevokeToken([FromBody] RefreshTokenDTO refreshTokenDto)
    {
        await _service.RevokeTokenAsync(refreshTokenDto.Token);
    }

    /// <summary>
    /// Generate reset password token and send email for reset password
    /// </summary>
    /// <param name="forgotPasswordDto">Email and callback url</param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
    {
        await _service.ForgotPassword(forgotPasswordDto);
    }

    /// <summary>
    /// Reset password by token
    /// </summary>
    /// <param name="resetPasswordDto">Credentials and reset password token</param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordViewModel>> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
    {
        ResetPasswordViewModel resetPasswordResult = await _service.ResetPassword(resetPasswordDto);
        return resetPasswordResult;
    }
}