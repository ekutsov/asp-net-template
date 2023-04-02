namespace EKUTSOV.Core.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly UserManager<User> _userManager;

    private readonly JWTSettings _jwtSettings;

    public AuthenticationService(AppDbContext context,
                                 IMapper mapper,
                                 UserManager<User> userManager,
                                 IOptions<JWTSettings> settings) : base(context, mapper)
    {
        _jwtSettings = settings.Value;
        _userManager = userManager;
    }

    public async Task<AuthenticateViewModel> AuthenticateAsync(AuthenticateDTO loginDto)
    {
        User dbUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.UserName == loginDto.Login || u.Email == loginDto.Login) && !u.IsDeleted);

        if (dbUser != null)
        {
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(dbUser, loginDto.Password);

            if (isPasswordCorrect)
            {
                IList<string> userRoles = await _userManager.GetRolesAsync(dbUser);

                string jwtToken = GenerateJwtToken(dbUser, userRoles);

                RefreshToken refreshToken = GenerateRefreshToken();

                refreshToken.UserId = dbUser.Id;

                _context.RefreshTokens.Add(refreshToken);

                await RemoveOldRefreshTokens(dbUser.Id);

                await _context.SaveChangesAsync();

                return new AuthenticateViewModel(jwtToken, refreshToken.Token);
            }
        }

        throw new ForbiddenException(AuthenticationErrors.LoginFailed);
    }

    /// <summary>
    /// Generate new jwt and refresh token
    /// </summary>
    /// <param name="token">refresh token</param>
    /// <returns>Access and refresh token</returns>
    /// <exception cref="ValidationException">Token expired or revoked</exception>
    public async Task<AuthenticateViewModel> RefreshTokenAsync(string token)
    {
        User user = await GetUserByRefreshToken(token);

        RefreshToken refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == token);

        if (refreshToken.RevokedDate.HasValue)
            RevokeDescendantRefreshTokens(refreshToken, user, $"Attempted reuse of revoked ancestor token: {token}");

        if (DateTime.UtcNow >= refreshToken.ExpiredDate || refreshToken.RevokedDate.HasValue)
            throw new BadRequestException(AuthenticationErrors.InvalidRefreshToken);

        RefreshToken newRefreshToken = RotateRefreshToken(refreshToken);

        newRefreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(newRefreshToken);

        await RemoveOldRefreshTokens(user.Id);

        await _context.SaveChangesAsync();

        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        string jwtToken = GenerateJwtToken(user, userRoles);

        return new AuthenticateViewModel(jwtToken, newRefreshToken.Token);
    }

    public async Task RevokeTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new BadRequestException(AuthenticationErrors.TokenRequired);
        }

        User user = await GetUserByRefreshToken(token);

        RefreshToken refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == token);

        if (DateTime.UtcNow >= refreshToken.ExpiredDate || refreshToken.RevokedDate.HasValue)
            throw new BadRequestException(AuthenticationErrors.InvalidRefreshToken);

        RevokeRefreshToken(refreshToken, "Revoked without replacement");

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Get email for reset password
    /// </summary>
    /// <param name="forgotPasswordDto">User name or email</param>
    /// <returns></returns>
    public async Task ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
    {
        User user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                (u.UserName == forgotPasswordDto.Login || u.Email == forgotPasswordDto.Login) && !u.IsDeleted);

        if (user == null)
        {
            throw new NotFoundException(AuthenticationErrors.UserDoesNotExists);
        }
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);

        string callbackUrlWithParams = AddParamsToCallbackUrl(forgotPasswordDto.CallbackUrl, token, user.Email);
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="resetPasswordInfo"></param>
    /// <returns></returns>
    public async Task<ResetPasswordViewModel> ResetPassword(ResetPasswordDTO resetPasswordInfo)
    {
        User user = await _userManager.FindByEmailAsync(resetPasswordInfo.Email);

        if (user == null || user.IsDeleted)
        {
            throw new NotFoundException(AuthenticationErrors.UserDoesNotExists);
        }

        IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(user,
                                                                                   resetPasswordInfo.Token,
                                                                                   resetPasswordInfo.Password);

        if (!resetPasswordResult.Succeeded)
        {
            throw new InvalidOperationException(AuthenticationErrors.ResetPasswordTokenExpired);
        }

        ResetPasswordViewModel result = new()
        {
            Email = resetPasswordInfo.Email,
            Success = resetPasswordResult.Succeeded
        };
        return result;
    }

    private static string AddParamsToCallbackUrl(string callbackUrl, string token, string email = "")
    {
        string urlEncodeToken = WebUtility.UrlEncode(token);
        string callbackUrlWithParams = $"{callbackUrl}?token={urlEncodeToken}";

        if (!string.IsNullOrEmpty(email))
        {
            string urlEncodeEmail = WebUtility.UrlEncode(email);
            callbackUrlWithParams = callbackUrlWithParams + $"&email={email}";
        }

        return callbackUrlWithParams;
    }

    private async Task<User> GetUserByRefreshToken(string token)
    {
        User user = await _context.Users
            .Where(user => !user.IsDeleted)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync(user => user.RefreshTokens.Any(rt => rt.Token == token));

        if (user == null)
        {
            throw new BadRequestException(AuthenticationErrors.InvalidRefreshToken);
        }

        return user;
    }

    private RefreshToken RotateRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken newRefreshToken = GenerateRefreshToken();

        RevokeRefreshToken(refreshToken, "Replaced by new token", newRefreshToken.Token);

        return newRefreshToken;
    }

    private async System.Threading.Tasks.Task RemoveOldRefreshTokens(int userId)
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-_jwtSettings.RefreshTokenExpireSeconds);
        List<RefreshToken> refreshTokens = await _context.RefreshTokens.Where(rt =>
                (DateTime.UtcNow >= rt.ExpiredDate || rt.RevokedDate.HasValue) && rt.CreatedDate <= date)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(refreshTokens);
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string reason)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            RefreshToken childToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken.ReplacedByToken);

            if (!(DateTime.UtcNow >= childToken.ExpiredDate) && !childToken.RevokedDate.HasValue)
                RevokeRefreshToken(childToken, reason);
            else
                RevokeDescendantRefreshTokens(childToken, user, reason);
        }
    }
    private static void RevokeRefreshToken(RefreshToken token, string reason = null, string replacedByToken = null)
    {
        token.RevokedDate = DateTime.UtcNow;
        token.RevokedReason = reason;
        token.ReplacedByToken = replacedByToken;
    }

    private string GenerateJwtToken(User user, IList<string> userRoles)
    {
        var unixStartDate = new DateTime(1970, 1, 1);

        long notValidBeforeTime = (long)DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5))
            .Subtract(unixStartDate).TotalSeconds;

        long expirationTime = (long)DateTime.UtcNow.AddSeconds(_jwtSettings.AccessTokenExpireSeconds)
            .Subtract(unixStartDate).TotalSeconds;

        JwtPayload payload = new()
        {
            { JwtRegisteredClaimNames.Sub, user.UserName },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            { JwtRegisteredClaimNames.Nbf, notValidBeforeTime },
            { JwtRegisteredClaimNames.Aud, _jwtSettings.Audience },
            { JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer },
            { JwtRegisteredClaimNames.Exp, expirationTime },
            { CustomClaims.UserId, user.Id.ToString() },
            { CustomClaims.Roles, userRoles }
        };

        byte[] jwtKey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        SymmetricSecurityKey key = new(jwtKey);

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtHeader header = new(creds);

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        string token = handler.WriteToken(securityToken);

        return token;
    }

    private RefreshToken GenerateRefreshToken()
    {
        DateTime dateNow = DateTime.UtcNow;

        DateTime expiredDate = dateNow.AddSeconds(_jwtSettings.RefreshTokenExpireSeconds);

        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[64];

        randomNumberGenerator.GetBytes(randomBytes);

        RefreshToken refreshToken = new()
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpiredDate = dateNow.AddSeconds(_jwtSettings.RefreshTokenExpireSeconds),
            CreatedDate = dateNow
        };

        return refreshToken;
    }
}