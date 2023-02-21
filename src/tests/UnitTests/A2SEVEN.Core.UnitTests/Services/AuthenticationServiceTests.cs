namespace A2SEVEN.Core.UnitTests.Services;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task AuthenticationService_AuthenticateAsync_ShouldReturnAuthenticationViewModel()
    {
        // Arrange
        var authDto = new AuthenticateDTO
        {
            Login = UserConstants.Login,
            Password = UserConstants.Password
        };

        var authViewModel = new AuthenticateViewModel(TokenConstants.JwtTokenTest, TokenConstants.RefreshTokenTest);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.AuthenticateAsync(authDto))
            .ReturnsAsync(authViewModel);

        // Act
        var result = await authenticationService.Object.AuthenticateAsync(authDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthenticateViewModel>();

        result.JwtToken.Should().BeEquivalentTo(authViewModel.JwtToken);
        result.RefreshToken.Should().BeEquivalentTo(authViewModel.RefreshToken);
    }

    [Fact]
    public async Task AuthenticationService_AuthenticateAsync_ShouldThrowForbiddenExceptionIfLoginFailed()
    {
        // Arrange
        var emptyAuthenticationDto = new AuthenticateDTO();

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.AuthenticateAsync(emptyAuthenticationDto))
            .ThrowsAsync(new ForbiddenException(AuthenticationErrors.LoginFailed));

        // Act
        var result = () => authenticationService.Object.AuthenticateAsync(emptyAuthenticationDto);

        // Assert
        await result.Should().ThrowAsync<ForbiddenException>()
            .WithMessage(AuthenticationErrors.LoginFailed);
    }

    [Fact]
    public async Task AuthenticationService_RefreshTokenAsync_ShouldReturnAuthenticateViewModel()
    {
        // Arrange
        var authenticationViewModel = new AuthenticateViewModel(TokenConstants.NewJwtTokenTest, TokenConstants.RefreshTokenTest);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RefreshTokenAsync(TokenConstants.RefreshTokenTest))
            .ReturnsAsync(authenticationViewModel);

        // Act
        var result = await authenticationService.Object.RefreshTokenAsync(TokenConstants.RefreshTokenTest);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthenticateViewModel>();
        result.JwtToken.Should().BeEquivalentTo(TokenConstants.NewJwtTokenTest);
        result.RefreshToken.Should().BeEquivalentTo(TokenConstants.RefreshTokenTest);
    }

    [Fact]
    public async Task AuthenticationService_RefreshTokenAsync_ThrowBadRequestExceptionIfTokenExpired()
    {
        // Arrange
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RefreshTokenAsync(TokenConstants.ExpiredTokenTest))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.InvalidRefreshToken));

        // Act
        var result = () => authenticationService.Object.RefreshTokenAsync(TokenConstants.ExpiredTokenTest);

        // Assert
        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task AuthenticationService_RevokeTokenAsync_NotTrowException()
    {
        // Arrange
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(TokenConstants.JwtTokenTest));

        // Act
        var result = () => authenticationService.Object.RevokeTokenAsync(TokenConstants.JwtTokenTest);

        // Assert
        await result.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthenticationService_RevokeTokenAsync_ThrowBadRequestExceptionIfTokenIsEmpty()
    {
        // Arrange
        var emptyToken = "";

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(emptyToken))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.TokenRequired));

        // Act
        var result = () => authenticationService.Object.RevokeTokenAsync(emptyToken);

        // Assert
        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.TokenRequired);
    }

    [Fact]
    public async Task AuthenticationService_RevokeTokenAsync_ThrowBadRequestExceptionIfTokenExpired()
    {
        // Arrange
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(TokenConstants.ExpiredTokenTest))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.TokenRequired));

        // Act
        var result = () => authenticationService.Object.RevokeTokenAsync(TokenConstants.ExpiredTokenTest);

        // Assert
        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.TokenRequired);
    }

    [Fact]
    public async Task AuthenticationService_ForgotPassword_ThrowNotFoundIfUserNotExist()
    {
        // Arrange
        var emptyForgotPasswordDto = new ForgotPasswordDTO();

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ForgotPassword(emptyForgotPasswordDto))
            .ThrowsAsync(new NotFoundException(AuthenticationErrors.UserDoesNotExists));

        // Act
        var result = () => authenticationService.Object.ForgotPassword(emptyForgotPasswordDto);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>()
            .WithMessage(AuthenticationErrors.UserDoesNotExists);
    }

    [Fact]
    public async Task AuthenticationService_ResetPassword_ReturnSuccessResetPasswordViewModel()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDTO
        {
            Email = UserConstants.Email,
            Password = UserConstants.Password,
            Token = TokenConstants.JwtTokenTest
        };

        var resetPasswordViewModel = new ResetPasswordViewModel
        {
            Email = UserConstants.Email,
            Success = true
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ResetPassword(resetPasswordDto))
            .ReturnsAsync(resetPasswordViewModel);

        // Act
        var result = await authenticationService.Object.ResetPassword(resetPasswordDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().NotBeNullOrEmpty();
        result.Email.Should().BeEquivalentTo(resetPasswordDto.Email);
        result.Success.Should().BeTrue();
        result.Success.Should().Be(resetPasswordViewModel.Success);
    }

    [Fact]
    public async Task AuthenticationService_ResetPassword_ReturnNotSuccessResetPasswordViewModel()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDTO
        {
            Email = UserConstants.Email,
            Password = UserConstants.Password,
            Token = TokenConstants.JwtTokenTest
        };

        var resetPasswordViewModel = new ResetPasswordViewModel
        {
            Email = UserConstants.Email,
            Success = false
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ResetPassword(resetPasswordDto))
            .ReturnsAsync(resetPasswordViewModel);

        // Act
        var result = await authenticationService.Object.ResetPassword(resetPasswordDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().NotBeNullOrEmpty();
        result.Email.Should().BeEquivalentTo(resetPasswordDto.Email);
        result.Success.Should().BeFalse();
        result.Success.Should().Be(resetPasswordViewModel.Success);
    }

    [Fact]
    public async Task AuthenticationService_ResetPassword_ThrowNotFoundExceptionIfUserNotFound()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDTO
        {
            Email = UserConstants.NotExistEmail,
            Password = UserConstants.Password,
            Token = TokenConstants.JwtTokenTest
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ResetPassword(resetPasswordDto))
            .ThrowsAsync(new NotFoundException(AuthenticationErrors.UserDoesNotExists));

        // Act
        var result = () => authenticationService.Object.ResetPassword(resetPasswordDto);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>()
            .WithMessage(AuthenticationErrors.UserDoesNotExists);
    }

    [Fact]
    public async Task AuthenticationService_ResetPassword_ThrowInvalidOperationIfResetPasswordFailed()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDTO
        {
            Email = UserConstants.Email,
            Password = UserConstants.Password,
            Token = TokenConstants.JwtTokenTest
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ResetPassword(resetPasswordDto))
            .ThrowsAsync(new InvalidOperationException(AuthenticationErrors.ResetPasswordTokenExpired));

        // Act
        var result = () => authenticationService.Object.ResetPassword(resetPasswordDto);

        // Assert
        await result.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(AuthenticationErrors.ResetPasswordTokenExpired);
    }
}