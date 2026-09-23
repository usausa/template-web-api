namespace Template.ApiServer.Host.Application.Authentication;

public sealed record LoginAccount(string Id, IReadOnlyList<string> Roles);

public interface ILoginProvider
{
    ValueTask<LoginAccount?> AuthenticateAsync(string id, string password, CancellationToken cancellationToken = default);
}
