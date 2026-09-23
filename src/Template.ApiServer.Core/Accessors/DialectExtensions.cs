namespace Template.ApiServer.Accessors;

public static class DialectExtensions
{
    public static string? Match(this IDialect dialect, string? keyword) =>
        String.IsNullOrWhiteSpace(keyword) ? null : $"%{dialect.LikeEscape(keyword.Trim())}%";
}
