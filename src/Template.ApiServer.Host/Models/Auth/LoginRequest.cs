namespace Template.ApiServer.Host.Models.Auth;

public sealed record LoginRequest(
    [property: Required] string Id,
    [property: Required] string Password);
