namespace A2SEVEN.Domain.Constants;

public static class AuthenticationErrors
{
    public const string LoginFailed = "Login failed! Incorrect user name or password";

    public const string InvalidRefreshToken = "Invalid refresh token";

    public const string EntityNotFound = "Entity not found";

    public const string EntityAlreadyExist = "Entity already exist";

    public const string TokenRequired = "Token is required";

    public const string UserDoesNotExists = "The user with this email or username does not exist!";

    public const string ResetPasswordTokenExpired = "Reset password token expired!";
}