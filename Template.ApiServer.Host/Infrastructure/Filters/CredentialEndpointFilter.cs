namespace Template.ApiServer.Host.Infrastructure.Filters;

using Template.ApiServer.Host.Application;

public sealed class CredentialEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // スキームごとにクレーム型が異なるため、ロールはIdentityのRoleClaimTypeで解決する
        if (context.HttpContext.User is { Identity: ClaimsIdentity { IsAuthenticated: true } identity } user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? identity.Name ?? string.Empty;
            var roles = user.FindAll(identity.RoleClaimType).Select(static x => x.Value).ToArray();
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
