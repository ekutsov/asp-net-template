namespace A2SEVEN.API.UnitTests.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task AuthenticationController_Authenticate_ShouldReturnActionResultWithAuthenticationViewModel()
    {
        // Arrange
        var authDto = new AuthenticateDTO
        {
            Login = UserConstants.Login,
            Password = UserConstants.Password
        };

        // For test refreshToken == jwtToken
        var authViewModel = new AuthenticateViewModel(TokenConstants.JwtTokenTest, TokenConstants.RefreshTokenTest);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.AuthenticateAsync(authDto))
            .ReturnsAsync(authViewModel);

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = await authController.Authenticate(authDto);

        // Assert
        result.Value.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AuthenticateViewModel>>();

        result.Value!.JwtToken.Should().BeEquivalentTo(authViewModel.JwtToken);
        result.Value!.RefreshToken.Should().BeEquivalentTo(authViewModel.RefreshToken);
    }

    [Fact]
    public async Task AuthenticationController_Authenticate_ShouldThrowForbiddenExceptionIfUserNotFound()
    {
        // Arrange
        var emptyAuthenticationDto = new AuthenticateDTO();

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.AuthenticateAsync(emptyAuthenticationDto))
            .ThrowsAsync(new ForbiddenException(AuthenticationErrors.LoginFailed));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.Authenticate(emptyAuthenticationDto);

        // Assert
        await result.Should().ThrowAsync<ForbiddenException>()
            .WithMessage(AuthenticationErrors.LoginFailed);
    }

    [Fact]
    public async Task AuthenticationController_RefreshToken_ReturnActionResultWithAuthenticateViewModelResult()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDTO
        {
            Token = TokenConstants.RefreshTokenTest
        };

        var authenticateViewModel = new AuthenticateViewModel(TokenConstants.NewJwtTokenTest, TokenConstants.RefreshTokenTest);

        var authenticationServie = new Mock<IAuthenticationService>();
        authenticationServie.Setup(e => e.RefreshTokenAsync(refreshTokenDto.Token))
            .ReturnsAsync(authenticateViewModel);

        var authController = new AuthenticationController(authenticationServie.Object);

        // Act
        var result = await authController.RefreshToken(refreshTokenDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AuthenticateViewModel>>();
        result.Value!.JwtToken.Should().BeEquivalentTo(TokenConstants.NewJwtTokenTest);
        result.Value!.RefreshToken.Should().BeEquivalentTo(TokenConstants.RefreshTokenTest);
    }

    [Fact]
    public async Task AuthenticationController_RefreshToken_ThrowBadRequestExceptionIfTokenExpired()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDTO
        {
            Token = TokenConstants.ExpiredTokenTest
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RefreshTokenAsync(refreshTokenDto.Token))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.InvalidRefreshToken));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.RefreshToken(refreshTokenDto);

        // Assert
        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task AuthenticationController_RevokeToken_NotTrowException()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDTO
        {
            Token = TokenConstants.JwtTokenTest
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(refreshTokenDto.Token));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.RevokeToken(refreshTokenDto);

        // Assert

        await result.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthenticationController_RevokeToken_ThrowBadRequestExceptionIfTokenIsEmpty()
    {
        // Arrange
        var emptyToken = "";

        var refreshTokenDto = new RefreshTokenDTO
        {
            Token = emptyToken
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(refreshTokenDto.Token))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.TokenRequired));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.RevokeToken(refreshTokenDto);

        // Assert
        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.TokenRequired);
    }

    [Fact]
    public async Task AuthenticationController_RevokeToken_ThrowBadReqeustExceptionIfTokenExpired()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDTO
        {
            Token = TokenConstants.ExpiredTokenTest
        };

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.RevokeTokenAsync(refreshTokenDto.Token))
            .ThrowsAsync(new BadRequestException(AuthenticationErrors.TokenRequired));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.RevokeToken(refreshTokenDto);

        // Assert

        await result.Should().ThrowAsync<BadRequestException>()
            .WithMessage(AuthenticationErrors.TokenRequired);
    }

    [Fact]
    public async Task AuthenticationController_ForgotPassword_ThrowNotFoundIfUserNotExists()
    {
        // Arrange
        var emptyForgotPasswordDto = new ForgotPasswordDTO();

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(e => e.ForgotPassword(emptyForgotPasswordDto))
            .ThrowsAsync(new NotFoundException(AuthenticationErrors.UserDoesNotExists));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.ForgotPassword(emptyForgotPasswordDto);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>()
            .WithMessage(AuthenticationErrors.UserDoesNotExists);
    }

    [Fact]
    public async Task AuthenticationController_ResetPassword_ReturnSuccessResetPasswordViewModel()
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

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = await authController.ResetPassword(resetPasswordDto);

        // Assert
        result.Should().NotBeNull();
        result.Value!.Email.Should().NotBeNullOrEmpty();
        result.Value!.Success.Should().BeTrue();
        result.Value!.Success.Should().Be(resetPasswordViewModel.Success);
    }

    [Fact]
    public async Task AuthenticationController_ResetPassword_ReturnNotSuccessResetPasswordViewModel()
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

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = await authController.ResetPassword(resetPasswordDto);

        // Assert
        result.Should().NotBeNull();
        result.Value!.Email.Should().NotBeNullOrEmpty();
        result.Value!.Email.Should().BeEquivalentTo(resetPasswordViewModel.Email);
        result.Value!.Success.Should().BeFalse();
        result.Value!.Success.Should().Be(resetPasswordViewModel.Success);
    }

    [Fact]
    public async Task AuthenticationController_ResetPassword_ThrowNotFoundExceptionIfUserNotFound()
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
            .ThrowsAsync(new NotFoundException(AuthenticationErrors.UserDoesNotExists));

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.ResetPassword(resetPasswordDto);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>()
            .WithMessage(AuthenticationErrors.UserDoesNotExists);
    }

    [Fact]
    public async Task AuthenticationController_ResetPassword_ThrowInvalidOperationIfResetPasswordFailed()
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

        var authController = new AuthenticationController(authenticationService.Object);

        // Act
        var result = () => authController.ResetPassword(resetPasswordDto);

        // Assert
        await result.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(AuthenticationErrors.ResetPasswordTokenExpired);
    }

}