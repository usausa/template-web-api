namespace Template.ApiServer.Host.Endpoints;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Infrastructure.Authentication;
using Template.ApiServer.Host.Models.Auth;

public static class AuthEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Auth)
            .RequireRateLimiting(RateLimitPolicies.Auth);

        group.MapPost("/login", HandleLoginAsync).AllowAnonymous();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleLoginAsync(
        LoginRequest request,
        ILoginProvider loginProvider,
        TokenService tokenService,
        CancellationToken cancellationToken)
    {
        var account = await loginProvider.AuthenticateAsync(request.Id, request.Password, cancellationToken);
        if (account is null)
        {
            return TypedResults.Unauthorized();
        }

        var (token, expireAt) = tokenService.CreateToken(account.Id, account.Roles);
        return TypedResults.Ok(new LoginResponse(token, expireAt));
    }
}
