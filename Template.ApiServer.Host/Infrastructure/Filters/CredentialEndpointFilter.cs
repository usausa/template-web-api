namespace Template.ApiServer.Host.Infrastructure.Filters;

using Template.ApiServer.Host.Application;

public sealed class CredentialEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated ?? false)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity.Name ?? string.Empty;
            var roles = user.FindAll(ClaimTypes.Role).Select(static x => x.Value).ToArray();
            CredentialContext.Current = new Credential(id, roles);
        }

        try
        {
            return await next(context);
        }
        finally
        {
            CredentialContext.Current = null;
        }
    }
}
