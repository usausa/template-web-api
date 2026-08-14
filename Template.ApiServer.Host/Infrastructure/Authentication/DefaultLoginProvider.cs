namespace Template.ApiServer.Host.Infrastructure.Authentication;

using Template.ApiServer.Host.Settings;

public sealed class DefaultLoginProvider : ILoginProvider
{
    private readonly AuthSetting setting;

    public DefaultLoginProvider(AuthSetting setting)
    {
        this.setting = setting;
    }

    public ValueTask<LoginAccount?> AuthenticateAsync(string id, string password, CancellationToken cancellationToken = default)
    {
        var user = setting.Users.Find(x => String.Equals(x.Id, id, StringComparison.Ordinal));
        if ((user is null) || !String.Equals(user.Password, password, StringComparison.Ordinal))
        {
            return ValueTask.FromResult<LoginAccount?>(null);
        }

        return ValueTask.FromResult<LoginAccount?>(new LoginAccount(user.Id, user.Roles));
    }
}
