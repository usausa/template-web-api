namespace Template.ApiServer.Host.Application;

public static class ApiRoutes
{
    // 版は URL のセグメント(/api/v1/...)
    private const string Prefix = "/api/v{version:apiVersion}";

    public const string Auth = Prefix + "/auth";

    public const string Data = Prefix + "/data";

    public const string Files = Prefix + "/files";

    public const string Test = Prefix + "/test";
}
