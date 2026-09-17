namespace Template.ApiServer.Host.Infrastructure.Logging;

public sealed class LoggingContextMiddleware
{
    private readonly RequestDelegate next;

    public LoggingContextMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        LoggingContext.Set(context.Connection.RemoteIpAddress?.ToString(), context.User.Identity?.Name);
        try
        {
            await next(context);
        }
        finally
        {
            LoggingContext.Clear();
        }
    }
}
