namespace A2SEVEN.Core.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            ExceptionResponse error = new(exception.Message);

            httpContext.Response.Clear();

            httpContext.Response.ContentType = "application/json";

            switch (exception)
            {
                case BadRequestException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ForbiddenException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                case NotFoundException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case UnauthorizedException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize<ExceptionResponse>(error));
        }
    }
}