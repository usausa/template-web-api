namespace Template.ApiServer.Settings;

using Template.ApiServer.Host.Settings;

public sealed class AuthSettingTests
{
    [Fact]
    public void ToStringMasksSecrets()
    {
        // Arrange
        var setting = new AuthSetting
        {
            SecretKey = "super-secret-key-0123456789abcdef",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpireMinutes = 60,
            ApiKey = "secret-api-key"
        };
        var user = new AuthSetting.UserEntry
        {
            Id = "test",
            Password = "secret-password"
        };
        user.Roles.Add("Administrator");
        setting.Users.Add(user);

        // Act
        var text = setting.ToString();

        // Assert
        Assert.Contains("TestIssuer", text, StringComparison.Ordinal);
        Assert.DoesNotContain("super-secret-key-0123456789abcdef", text, StringComparison.Ordinal);
        Assert.DoesNotContain("secret-api-key", text, StringComparison.Ordinal);
        Assert.DoesNotContain("secret-password", text, StringComparison.Ordinal);
    }
}
