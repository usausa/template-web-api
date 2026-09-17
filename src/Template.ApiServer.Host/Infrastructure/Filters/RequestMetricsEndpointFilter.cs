namespace Template.ApiServer.Host.Infrastructure.Filters;

using Template.ApiServer.Host.Application.Telemetry;

public sealed class RequestMetricsEndpointFilter : IEndpointFilter
{
    private readonly ILogger<RequestMetricsEndpointFilter> log;

    private readonly TimeProvider timeProvider;

    private readonly TimeSpan longExecutionThreshold;

    private readonly ApplicationInstrument instrument;

    public RequestMetricsEndpointFilter(
        ILogger<RequestMetricsEndpointFilter> log,
        TimeProvider timeProvider,
        TelemetrySetting setting,
        ApplicationInstrument instrument)
    {
        this.log = log;
        this.timeProvider = timeProvider;
        longExecutionThreshold = TimeSpan.FromMilliseconds(setting.LongExecutionThreshold);
        this.instrument = instrument;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var route = (context.HttpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText ?? context.HttpContext.Request.Path.Value ?? string.Empty;
        instrument.IncrementRequestExecution(method, route);

        var start = timeProvider.GetTimestamp();
        try
        {
            return await next(context);
        }
        finally
        {
            var elapsed = timeProvider.GetElapsedTime(start);
            if (elapsed >= longExecutionThreshold)
            {
                instrument.IncrementRequestLongExecution(method, route);
                log.WarnLongExecution(method, route, (long)elapsed.TotalMilliseconds);
            }
        }
    }
}
