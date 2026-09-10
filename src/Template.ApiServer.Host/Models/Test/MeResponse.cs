namespace Template.ApiServer.Host.Models.Test;

public sealed record MeResponse(string Id, IReadOnlyList<string> Roles);
