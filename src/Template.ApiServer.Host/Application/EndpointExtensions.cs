namespace Template.ApiServer.Host.Application;

using Template.ApiServer.Host.Infrastructure.Filters;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix) =>
        endpoints.MapGroup(prefix).AddEndpointFilter<RequestMetricsEndpointFilter>();
}
