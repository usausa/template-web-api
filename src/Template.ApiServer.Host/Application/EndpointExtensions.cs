namespace Template.ApiServer.Host.Application;

using Template.ApiServer.Host.Application.Context;
using Template.ApiServer.Host.Application.Telemetry;

public static class EndpointExtensions
{
    // v1 だけを定義する。版を足すときは HasApiVersion を重ね、旧版は HasDeprecatedApiVersion にする
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix) =>
        endpoints.NewVersionedApi()
            .MapGroup(prefix)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<RequestMetricsEndpointFilter>()
            .AddEndpointFilter<ServiceContextEndpointFilter>();
}
