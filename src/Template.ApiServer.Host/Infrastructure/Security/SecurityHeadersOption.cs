namespace Template.ApiServer.Host.Infrastructure.Security;

public sealed class SecurityHeadersOption
{
    // Content-Security-Policy is only reported, not enforced
    public bool ReportOnly { get; set; }

    // Content-Security-Policy value. null = no CSP header
    public string? ContentSecurityPolicy { get; set; }
}
