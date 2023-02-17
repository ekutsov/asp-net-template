using System.Net;

namespace A2SEVEN.API.UnitTests.Controllers
{
    public class AuthenticationControllerTests
    {
        [Fact]
        public async Task AuthenticationController_Authenticate_ShouldReturnActionResultWithAuthenticationViewModel()
        {
            // Arrange
            var authDto = new AuthenticateDTO
            {
                Login = "login",
                Password = "password"
            };

            var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            // For test make refreshToken == jwtToken
            var refreshTokenExample = jwtTokenExample;

            var authViewModel = new AuthenticateViewModel(jwtTokenExample, refreshTokenExample);

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
            var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var refreshToken = jwtTokenExample;

            var refreshTokenDto = new RefreshTokenDTO
            {
                Token = jwtTokenExample
            };

            var newToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NzAxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.Ht_Bcj-vgcpafoa0RUkkh_IQVKivx7AzLq2HqKNagGs";

            var authenticateViewModel = new AuthenticateViewModel(newToken, refreshToken);

            var authenticationServie = new Mock<IAuthenticationService>();
            authenticationServie.Setup(e => e.RefreshTokenAsync(refreshTokenDto.Token))
                .ReturnsAsync(authenticateViewModel);

            var authController = new AuthenticationController(authenticationServie.Object);

            // Act
            var result = await authController.RefreshToken(refreshTokenDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<AuthenticateViewModel>>();
            result.Value!.JwtToken.Should().BeEquivalentTo(newToken);
            result.Value!.RefreshToken.Should().BeEquivalentTo(refreshToken);
        }

        [Fact]
        public async Task AuthenticationController_RefreshToken_ThrowBadRequestExceptionIfTokenExpired() 
        {
            // Arrange
            var expiredToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTY3NjYzMTAxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.GROd-k6pW4pIIUFAr_jhegcmpSckYD0nzhe0RRKgcSg";

            var refreshTokenDto = new RefreshTokenDTO 
            {
                Token = expiredToken
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
             var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var refreshTokenDto = new RefreshTokenDTO 
            {
                Token = jwtTokenExample
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
        public async Task AuthenticationController_RevokeToken_ThrowBadReqeustExceptionIfTokenIsExpired() 
        {
            // Arrange
            var expiredToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTY3NjYzMTAxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.GROd-k6pW4pIIUFAr_jhegcmpSckYD0nzhe0RRKgcSg";

            var refreshTokenDto = new RefreshTokenDTO
            {
                Token = expiredToken
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
        public async Task AuthenticationController_ResetPasword_ReturnSuccessResetPasswordViewModel() 
        {
             var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var email = "email@email.com";

            // Arrange
            var resetPasswordDto = new ResetPasswordDTO 
            {
                Email = email,
                Password = "password",
                Token = jwtTokenExample
            };

            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Email = email,
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
        public async Task AuthenticationController_ResetPasword_ReturnNotSuccessResetPasswordViewModel() 
        {
             var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var email = "email@email.com";

            // Arrange
            var resetPasswordDto = new ResetPasswordDTO 
            {
                Email = email,
                Password = "password",
                Token = jwtTokenExample
            };

            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Email = email,
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
            var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var notExistEmail = "email@email.com";

            var resetPasswordDto = new ResetPasswordDTO 
            {
                Email = notExistEmail,
                Password = "password",
                Token = jwtTokenExample
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
            var jwtTokenExample =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzY2MzA3MTYsImV4cCI6MTcwODE2NjcxNiwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoibG9naW4iLCJHaXZlbk5hbWUiOiJMb2dpbiJ9.kENlR-MCskBWV9Tp5B4VA77PEfAv3vt6LfsbER2WOs4";

            var notExistEmail = "email@email.com";

            var resetPasswordDto = new ResetPasswordDTO 
            {
                Email = notExistEmail,
                Password = "password",
                Token = jwtTokenExample
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
}