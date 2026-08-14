namespace Template.ApiServer.Host.Application;

public sealed record Credential(string Id, IReadOnlyList<string> Roles);

public static class CredentialContext
{
    private static readonly AsyncLocal<Credential?> Local = new();

    public static Credential? Current
    {
        get => Local.Value;
        set => Local.Value = value;
    }
}
