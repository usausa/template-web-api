namespace Template.ApiServer.Host.Application.Authentication;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "ApiKey";

    public const string HeaderName = "X-Api-Key";

    public string ApiKey { get; set; } = default!;
}
