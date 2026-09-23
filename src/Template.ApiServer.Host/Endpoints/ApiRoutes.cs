namespace Template.ApiServer.Host.Endpoints;

public static class ApiRoutes
{
    public const string Prefix = "/api";

    // 版は URL のセグメント(/api/v1/...)
    private const string VersionedPrefix = Prefix + "/v{version:apiVersion}";

    public const string Auth = VersionedPrefix + "/auth";

    public const string Data = VersionedPrefix + "/data";

    public const string Files = VersionedPrefix + "/files";

    public const string Test = VersionedPrefix + "/test";
}
