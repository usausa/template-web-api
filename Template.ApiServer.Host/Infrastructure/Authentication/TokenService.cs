namespace Template.ApiServer.Host.Infrastructure.Authentication;

using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using Template.ApiServer.Host.Settings;

public sealed class TokenService
{
    private readonly AuthSetting setting;

    private readonly TimeProvider timeProvider;

    private readonly SigningCredentials credentials;

    public TokenService(AuthSetting setting, TimeProvider timeProvider)
    {
        this.setting = setting;
        this.timeProvider = timeProvider;
        credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey)), SecurityAlgorithms.HmacSha256);
    }

    public (string Token, DateTimeOffset ExpireAt) CreateToken(string id, IReadOnlyList<string> roles)
    {
        var now = timeProvider.GetUtcNow();
        var expireAt = now.AddMinutes(setting.ExpireMinutes);

        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, id) };
        claims.AddRange(roles.Select(static x => new Claim(ClaimTypes.Role, x)));

        var token = new JwtSecurityToken(
            issuer: setting.Issuer,
            audience: setting.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expireAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expireAt);
    }
}
