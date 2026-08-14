namespace Template.ApiServer.Host.Models.File;

public sealed record FileListResponse(IReadOnlyList<string> Entries);
