namespace Template.ApiServer.Host.Application.Context;

public static class HttpServiceContext
{
    private const string ServiceContextKey = "__ServiceContext";

    private const string AnonymousUserId = "anonymous";

    public static ServiceContext GetOrCreate(HttpContext httpContext)
    {
        if (httpContext.Items[ServiceContextKey] is ServiceContext existing)
        {
            return existing;
        }

        var context = Create(httpContext);
        httpContext.Items[ServiceContextKey] = context;

        return context;
    }

    public static ServiceContext Create(HttpContext httpContext) =>
        new(httpContext.RequestServices.GetRequiredService<TimeProvider>().GetLocalNow(), GetUserId(httpContext.User));

    // スキームごとにクレーム型が異なるため、利用者は NameIdentifier → Identity.Name の順に解決する
    public static string GetUserId(ClaimsPrincipal? user)
    {
        var identity = user?.Identity;
        if (identity?.IsAuthenticated ?? false)
        {
            var id = user!.FindFirstValue(ClaimTypes.NameIdentifier) ?? identity.Name;
            if (!String.IsNullOrEmpty(id))
            {
                return id;
            }
        }

        return AnonymousUserId;
    }
}
