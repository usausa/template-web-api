namespace Template.ApiServer.Application.Authentication;

using Template.ApiServer.Host.Application.Authentication;

public sealed class DefaultLoginProviderTests
{
    [Fact]
    public async Task AuthenticateReturnsAccount()
    {
        // Arrange
        var provider = new DefaultLoginProvider(CreateSetting());

        // Act
        var account = await provider.AuthenticateAsync("test", "test", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(account);
        Assert.Equal("test", account.Id);
        Assert.Contains("Administrator", account.Roles);
    }

    [Fact]
    public async Task AuthenticateWithWrongPasswordReturnsNull()
    {
        // Arrange
        var provider = new DefaultLoginProvider(CreateSetting());

        // Act
        var account = await provider.AuthenticateAsync("test", "wrong", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(account);
    }

    [Fact]
    public async Task AuthenticateWithUnknownIdReturnsNull()
    {
        // Arrange
        var provider = new DefaultLoginProvider(CreateSetting());

        // Act
        var account = await provider.AuthenticateAsync("unknown", "test", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(account);
    }

    private static AuthSetting CreateSetting()
    {
        var setting = new AuthSetting
        {
            SecretKey = "unit-test-secret-key-0123456789abcdef",
            Issuer = "Test",
            Audience = "Test",
            ExpireMinutes = 60,
            ApiKey = "unit-test-api-key"
        };
        var user = new LoginUserOption
        {
            Id = "test",
            Password = "test"
        };
        user.Roles.Add("Administrator");
        setting.Users.Add(user);
        return setting;
    }
}
