namespace Template.ApiServer.Host.Models.Data;

public sealed record DataUpdateRequest(
    [property: Required][property: MaxLength(50)] string Name,
    [property: Range(0, 1_000_000)] int Value);
