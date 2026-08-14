namespace Template.ApiServer.Host.Models.Auth;

public sealed record LoginResponse(string Token, DateTimeOffset ExpireAt);
