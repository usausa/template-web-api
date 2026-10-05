namespace Template.ApiServer.Host.Endpoints;

using Microsoft.FeatureManagement;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Application.Context;

//--------------------------------------------------------------------------------
// Models
//--------------------------------------------------------------------------------

public sealed class TestTimeResponse
{
    public DateTimeOffset Time { get; set; }
}

public sealed class TestFeatureResponse
{
    public bool Enabled { get; set; }
}

public sealed class TestMeResponse
{
    public string Id { get; set; } = default!;

    public IReadOnlyList<string> Roles { get; set; } = default!;
}

//--------------------------------------------------------------------------------
// Endpoints
//--------------------------------------------------------------------------------

public static class TestEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapTestEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Test);

        group.MapGet("/time", HandleTime)
            .WithName("TestTime");
        group.MapGet("/feature", HandleFeatureAsync)
            .WithName("TestFeature");
        group.MapGet("/error", HandleError)
            .WithName("TestError");
        group.MapGet("/me", HandleMe)
            .RequireAuthorization()
            .WithName("TestMe")
            .Produces<TestMeResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    //--------------------------------------------------------------------------------
    // Time
    //--------------------------------------------------------------------------------

    private static Ok<TestTimeResponse> HandleTime(TimeProvider timeProvider) =>
        TypedResults.Ok(new TestTimeResponse { Time = timeProvider.GetLocalNow() });

    //--------------------------------------------------------------------------------
    // Feature
    //--------------------------------------------------------------------------------

    private static async ValueTask<Ok<TestFeatureResponse>> HandleFeatureAsync(IFeatureManager featureManager) =>
        TypedResults.Ok(new TestFeatureResponse { Enabled = await featureManager.IsEnabledAsync(FeatureFlags.CustomOption) });

    //--------------------------------------------------------------------------------
    // Error
    //--------------------------------------------------------------------------------

    private static IResult HandleError() =>
        throw new InvalidOperationException("Test exception.");

    //--------------------------------------------------------------------------------
    // Me
    //--------------------------------------------------------------------------------

    // スキームごとにクレーム型が異なるため、ロールは Identity の RoleClaimType で解決する
    private static IResult HandleMe(ClaimsPrincipal user)
    {
        if (user.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(new TestMeResponse
        {
            Id = HttpServiceContext.GetUserId(user),
            Roles = user.FindAll(identity.RoleClaimType).Select(static x => x.Value).ToArray()
        });
    }
}
