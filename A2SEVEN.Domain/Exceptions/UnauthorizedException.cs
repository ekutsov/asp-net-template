namespace A2SEVEN.Domain.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? message) : base(message) { }
}