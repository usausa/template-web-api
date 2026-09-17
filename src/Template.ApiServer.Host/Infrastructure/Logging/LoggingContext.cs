namespace Template.ApiServer.Host.Infrastructure.Logging;

public static class LoggingContext
{
    private static readonly AsyncLocal<LoggingContextData?> Local = new();

    public static string? RemoteIpAddress => Local.Value?.RemoteIpAddress;

    public static string? UserId => Local.Value?.UserId;

    public static void Set(string? remoteIpAddress, string? userId) =>
        Local.Value = new LoggingContextData(remoteIpAddress, userId);

    public static void Clear() =>
        Local.Value = null;
}
