namespace A2SEVEN.Domain.Exceptions;

public class ExceptionResponse
{
    public ExceptionResponse(string? message)
    {
        Message = message;
    }

    public string? Message { get; protected set; }
}