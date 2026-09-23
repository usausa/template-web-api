namespace Template.ApiServer.Host.Endpoints;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Application.Authentication;

//--------------------------------------------------------------------------------
// Models
//--------------------------------------------------------------------------------

public sealed class LoginRequest
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}

public sealed class LoginResponse
{
    public string Token { get; set; } = default!;

    public DateTimeOffset ExpireAt { get; set; }
}

//--------------------------------------------------------------------------------
// Endpoints
//--------------------------------------------------------------------------------

public static class AuthEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Auth)
            .RequireRateLimiting(RateLimitPolicies.Auth);

        group.MapPost("/login", HandleLoginAsync).AllowAnonymous();
    }

    //--------------------------------------------------------------------------------
    // Login
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleLoginAsync(
        LoginRequest request,
        ILoginProvider loginProvider,
        JwtTokenProvider tokenProvider,
        CancellationToken cancellationToken)
    {
        var account = await loginProvider.AuthenticateAsync(request.Id, request.Password, cancellationToken);
        if (account is null)
        {
            return TypedResults.Unauthorized();
        }

        var (token, expireAt) = tokenProvider.CreateToken(account.Id, account.Roles);
        return TypedResults.Ok(new LoginResponse { Token = token, ExpireAt = expireAt });
    }
}
