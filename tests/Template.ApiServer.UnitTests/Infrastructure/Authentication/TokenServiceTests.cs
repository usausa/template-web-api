namespace Template.ApiServer.Infrastructure.Authentication;

using Template.ApiServer.Host.Infrastructure.Authentication;
using Template.ApiServer.Host.Settings;

public sealed class TokenServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 23, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateTokenReturnsTokenWithExpire()
    {
        // Arrange
        var service = new TokenService(CreateSetting(), new TestTimeProvider(Now));

        // Act
        var (token, expireAt) = service.CreateToken("test", ["Administrator"]);

        // Assert
        Assert.Equal(3, token.Split('.').Length);
        Assert.Equal(Now.AddMinutes(60), expireAt);
    }

    [Fact]
    public void CreateTokenContainsSubject()
    {
        // Arrange
        var service = new TokenService(CreateSetting(), new TestTimeProvider(Now));

        // Act
        var (token, _) = service.CreateToken("test", ["Administrator"]);
        var payload = DecodePayload(token);

        // Assert
        Assert.Contains("\"sub\":\"test\"", payload, StringComparison.Ordinal);
    }

    private static AuthSetting CreateSetting() =>
        new()
        {
            SecretKey = "unit-test-secret-key-0123456789abcdef",
            Issuer = "Test",
            Audience = "Test",
            ExpireMinutes = 60,
            ApiKey = "unit-test-api-key"
        };

    private static string DecodePayload(string token)
    {
        var value = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
        return Encoding.UTF8.GetString(Convert.FromBase64String(value.PadRight(value.Length + ((4 - (value.Length % 4)) % 4), '=')));
    }

    private sealed class TestTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset now;

        public TestTimeProvider(DateTimeOffset now)
        {
            this.now = now;
        }

        public override DateTimeOffset GetUtcNow() => now;
    }
}
