namespace Template.ApiServer.Host.Infrastructure.Security;

// Security headers for every response
public sealed class SecurityHeadersMiddleware
{
    private static readonly Func<object, Task> OnStartingCallback = OnStarting;

    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(OnStartingCallback, context);
        return next(context);
    }

    private static Task OnStarting(object state)
    {
        var headers = ((HttpContext)state).Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        return Task.CompletedTask;
    }
}
