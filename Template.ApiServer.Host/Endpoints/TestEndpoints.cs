namespace Template.ApiServer.Host.Endpoints;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Infrastructure.Filters;
using Template.ApiServer.Host.Models.Test;

public static class TestEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapTestEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Test);

        group.MapGet("/time", HandleTime);
        group.MapGet("/error", HandleError);
        group.MapGet("/me", HandleMe)
            .RequireAuthorization()
            .AddEndpointFilter<CredentialEndpointFilter>();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static Ok<TimeResponse> HandleTime(TimeProvider timeProvider) =>
        TypedResults.Ok(new TimeResponse(timeProvider.GetLocalNow()));

    private static IResult HandleError() =>
        throw new InvalidOperationException("Test exception.");

    private static IResult HandleMe()
    {
        var credential = CredentialContext.Current;
        return credential is not null
            ? TypedResults.Ok(new MeResponse(credential.Id, credential.Roles))
            : TypedResults.Unauthorized();
    }
}
