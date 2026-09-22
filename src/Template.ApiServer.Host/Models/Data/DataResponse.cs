namespace Template.ApiServer.Host.Models.Data;

public sealed record DataResponse(long Id, string Name, int Value, int Version, DateTime CreatedAt);
