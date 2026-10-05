namespace Template.ApiServer.Host.Application;

using Template.ApiServer.Host.Application.Context;
using Template.ApiServer.Host.Application.Telemetry;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix) =>
        endpoints.NewVersionedApi()
            .MapGroup(prefix)
            .HasApiVersion(ApiVersions.V1)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<RequestMetricsEndpointFilter>()
            .AddEndpointFilter<ServiceContextEndpointFilter>();
}
