namespace Template.ApiServer.Host.Infrastructure.Security;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    private readonly SecurityHeadersOption option;

    private readonly Func<object, Task> onStarting;

    public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersOption option)
    {
        this.next = next;
        this.option = option;
        onStarting = OnStarting;
    }

    public Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(onStarting, context);
        return next(context);
    }

    private Task OnStarting(object state)
    {
        var headers = ((HttpContext)state).Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        if (option.ContentSecurityPolicy is not null)
        {
            if (option.ReportOnly)
            {
                headers.ContentSecurityPolicyReportOnly = option.ContentSecurityPolicy;
            }
            else
            {
                headers.ContentSecurityPolicy = option.ContentSecurityPolicy;
            }
        }

        return Task.CompletedTask;
    }
}
