namespace Template.ApiServer.Host.Application.Authentication;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

// JWT の発行 (検証側の設定 AuthSetting と同じ値を使う)
public sealed class JwtTokenProvider
{
    private static readonly JsonWebTokenHandler Handler = new();

    private readonly AuthSetting setting;

    private readonly TimeProvider timeProvider;

    private readonly SigningCredentials credentials;

    public JwtTokenProvider(AuthSetting setting, TimeProvider timeProvider)
    {
        this.setting = setting;
        this.timeProvider = timeProvider;
        credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey)), SecurityAlgorithms.HmacSha256);
    }

    public (string Token, DateTimeOffset ExpireAt) CreateToken(string id, IReadOnlyList<string> roles)
    {
        var now = timeProvider.GetUtcNow();
        var expireAt = now.AddMinutes(setting.ExpireMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = setting.Issuer,
            Audience = setting.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expireAt.UtcDateTime,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = id,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString("N"),
                ["role"] = roles
            },
            SigningCredentials = credentials
        };

        return (Handler.CreateToken(descriptor), expireAt);
    }
}
